using BuysimTechnology.Data;
using BuysimTechnology.Models;
using BuysimTechnology.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Services
{
    public class GateService
    {
        private readonly AppDbContext _db;

        public GateService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GateResultVm> ValidateTicketAsync(string ticketNumber)
        {
            var invitation = await _db.Invitations
                .Include(i => i.Merchant)
                .FirstOrDefaultAsync(i => i.TicketNumber == ticketNumber.Trim().ToUpper());

            if (invitation == null)
            {
                return new GateResultVm
                {
                    IsValid = false,
                    Message = "Ticket not found. Please check the number and try again.",
                    TicketNumber = ticketNumber
                };
            }

            if (invitation.Status == InvitationStatus.Removed)
            {
                return new GateResultVm
                {
                    IsValid = false,
                    Message = "This invitation has been cancelled and is no longer valid.",
                    GuestName = invitation.GuestName,
                    MerchantName = invitation.Merchant.Name,
                    TicketNumber = ticketNumber,
                    Status = "Cancelled"
                };
            }

            if (invitation.Status == InvitationStatus.CheckedIn)
            {
                return new GateResultVm
                {
                    IsValid = false,
                    Message = $"This ticket was already used for check-in at {invitation.CheckedInAt:HH:mm}.",
                    GuestName = invitation.GuestName,
                    MerchantName = invitation.Merchant.Name,
                    TicketNumber = ticketNumber,
                    Status = "Already Checked In"
                };
            }

            // Valid — mark as checked in
            invitation.Status = InvitationStatus.CheckedIn;
            invitation.CheckedInAt = DateTime.UtcNow;

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = invitation.MerchantId,
                Action = "CheckedIn",
                Details = $"Guest '{invitation.GuestName}' checked in at gate. Ticket: {ticketNumber}",
                PerformedBy = "GateStaff"
            });

            await _db.SaveChangesAsync();

            return new GateResultVm
            {
                IsValid = true,
                Message = "Access granted. Welcome to the event!",
                GuestName = invitation.GuestName,
                MerchantName = invitation.Merchant.Name,
                TicketNumber = ticketNumber,
                Status = "Checked In"
            };
        }
    }
}
