using BuysimTechnology.Data;
using BuysimTechnology.Models;
using BuysimTechnology.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Services
{
    public class InvitationService
    {
        private readonly AppDbContext _db;
        private readonly QrCodeService _qrService;
        private readonly SecureTicketService _ticketService;

        public InvitationService(AppDbContext db, QrCodeService qrService, SecureTicketService ticketService)
        {
            _db = db;
            _qrService = qrService;
            _ticketService = ticketService;
        }

        public async Task<(bool Success, string Error)>
            AddGuestAsync(GuestFormVm form)
        {
            var merchant = await _db.Merchants
                .Include(m => m.Invitations)
                .FirstOrDefaultAsync(m => m.MerchantId == form.MerchantId);

            if (merchant == null)
                return (false, "Merchant not found.");

            var activeCount = merchant.Invitations
                .Count(i => i.Status != InvitationStatus.Removed);

            if (activeCount >= merchant.MaxInvites)
                return (false, $"Guest limit reached. Maximum {merchant.MaxInvites} guests allowed.");

            var duplicate = merchant.Invitations
                .Any(i => i.GuestEmail == form.GuestEmail
                       && i.Status != InvitationStatus.Removed);

            if (duplicate)
                return (false, "This email is already registered for this merchant.");

            // Generate secure, non-sequential ticket number — BSM-XXXXXXXXXX format
            // Using cryptographically secure random generation instead of sequential numbers
            var ticketNumber = await _ticketService.GenerateSecureTicketAsync();

            // Generate QR code containing the ticket number
            var qrBase64 = _qrService.GenerateQrCodeBase64(ticketNumber);

            var invitation = new Invitation
            {
                MerchantId = form.MerchantId,
                GuestName = form.GuestName,
                GuestEmail = form.GuestEmail,
                GuestPhone = form.GuestPhone,
                GuestIdNumber = form.GuestIdNumber,
                TicketNumber = ticketNumber,
                QrCodeBase64 = qrBase64,
                Status = InvitationStatus.Pending
            };

            _db.Invitations.Add(invitation);

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = form.MerchantId,
                Action = "GuestAdded",
                Details = $"Added: {form.GuestName} ({form.GuestEmail}) — Ticket: {ticketNumber}",
                PerformedBy = merchant.Name
            });

            await _db.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)>
            EditGuestAsync(GuestFormVm form)
        {
            var invitation = await _db.Invitations.FindAsync(form.InvitationId);
            if (invitation == null)
                return (false, "Guest not found.");

            if (invitation.Status == InvitationStatus.Removed)
                return (false, "Cannot edit a removed guest.");

            var oldName = invitation.GuestName;
            var oldEmail = invitation.GuestEmail;

            invitation.GuestName = form.GuestName;
            invitation.GuestEmail = form.GuestEmail;
            invitation.GuestPhone = form.GuestPhone;
            invitation.GuestIdNumber = form.GuestIdNumber;
            invitation.UpdatedAt = DateTime.UtcNow;

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = invitation.MerchantId,
                Action = "GuestEdited",
                Details = $"Updated: {oldName} ({oldEmail}) → {form.GuestName} ({form.GuestEmail})",
                PerformedBy = "Merchant"
            });

            await _db.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)>
            RemoveGuestAsync(int invitationId)
        {
            var invitation = await _db.Invitations.FindAsync(invitationId);
            if (invitation == null)
                return (false, "Guest not found.");

            if (invitation.Status == InvitationStatus.Removed)
                return (false, "Guest already removed.");

            invitation.Status = InvitationStatus.Removed;
            invitation.UpdatedAt = DateTime.UtcNow;

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = invitation.MerchantId,
                Action = "GuestRemoved",
                Details = $"Removed: {invitation.GuestName} ({invitation.GuestEmail}) — Ticket: {invitation.TicketNumber}",
                PerformedBy = "Merchant"
            });

            await _db.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}
