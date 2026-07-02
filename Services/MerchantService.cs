using BuysimTechnology.Data;
using BuysimTechnology.Models;
using BuysimTechnology.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Services
{
    public class MerchantService
    {
        private readonly AppDbContext _db;

        public MerchantService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Merchant>> GetAllAsync()
        {
            return await _db.Merchants
                .Include(m => m.Invitations)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Merchant?> GetByIdAsync(int id)
        {
            return await _db.Merchants
                .Include(m => m.Invitations)
                .Include(m => m.AuditLogs)
                .FirstOrDefaultAsync(m => m.MerchantId == id);
        }

        public async Task<Merchant?> GetByTokenAsync(string token)
        {
            return await _db.Merchants
                .Include(m => m.Invitations)
                .FirstOrDefaultAsync(m => m.PortalToken == token && m.IsActive);
        }

        public async Task<Merchant> CreateAsync(MerchantFormVm form)
        {
            var merchant = new Merchant
            {
                Name = form.Name,
                ContactEmail = form.ContactEmail,
                ContactPhone = form.ContactPhone,
                MaxInvites = form.MaxInvites,
                // Generate a unique token for this merchant's portal link
                PortalToken = Guid.NewGuid().ToString("N")[..12]
            };

            _db.Merchants.Add(merchant);
            await _db.SaveChangesAsync();  // Save merchant first to get MerchantId

            // Now add audit log with correct MerchantId
            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = merchant.MerchantId,
                Action = "Created",
                Details = $"Merchant '{merchant.Name}' created with limit of {merchant.MaxInvites} guests.",
                PerformedBy = "Admin"
            });

            await _db.SaveChangesAsync();
            return merchant;
        }

        public async Task<bool> UpdateAsync(MerchantFormVm form)
        {
            var merchant = await _db.Merchants.FindAsync(form.MerchantId);
            if (merchant == null) return false;

            merchant.Name = form.Name;
            merchant.ContactEmail = form.ContactEmail;
            merchant.ContactPhone = form.ContactPhone;
            merchant.MaxInvites = form.MaxInvites;

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = merchant.MerchantId,
                Action = "Updated",
                Details = $"Merchant details updated. Guest limit set to {form.MaxInvites}.",
                PerformedBy = "Admin"
            });

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int merchantId)
        {
            var merchant = await _db.Merchants.FindAsync(merchantId);
            if (merchant == null) return false;

            merchant.IsActive = !merchant.IsActive;

            _db.AuditLogs.Add(new AuditLog
            {
                MerchantId = merchant.MerchantId,
                Action = merchant.IsActive ? "Activated" : "Deactivated",
                Details = $"Merchant '{merchant.Name}' was {(merchant.IsActive ? "activated" : "deactivated")}.",
                PerformedBy = "Admin"
            });

            await _db.SaveChangesAsync();
            return true;
        }

        public int GetActiveGuestCount(Merchant merchant)
        {
            return merchant.Invitations
                .Count(i => i.Status != InvitationStatus.Removed);
        }
    }
}
