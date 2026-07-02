using BuysimTechnology.Data;
using BuysimTechnology.Models;
using BuysimTechnology.Models.ViewModels;
using BuysimTechnology.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Controllers
{
    public class AdminController : Controller
    {
        private readonly MerchantService _merchantService;
        private readonly InvitationService _invitationService;
        private readonly AppDbContext _db;

        public AdminController(MerchantService merchantService,
                               InvitationService invitationService,
                               AppDbContext db)
        {
            _merchantService = merchantService;
            _invitationService = invitationService;
            _db = db;
        }

        // GET /admin
        public async Task<IActionResult> Index()
        {
            var merchants = await _merchantService.GetAllAsync();

            var vm = new AdminDashboardVm
            {
                TotalMerchants = merchants.Count,
                TotalGuests = merchants.Sum(m => m.Invitations
                    .Count(i => i.Status != InvitationStatus.Removed)),
                TotalCheckedIn = merchants.Sum(m => m.Invitations
                    .Count(i => i.Status == InvitationStatus.CheckedIn)),
                TotalPending = merchants.Sum(m => m.Invitations
                    .Count(i => i.Status == InvitationStatus.Pending)),
                RecentMerchants = merchants.Take(5).ToList()
            };

            return View(vm);
        }

        // GET /admin/merchants
        public async Task<IActionResult> Merchants()
        {
            var merchants = await _merchantService.GetAllAsync();
            return View(merchants);
        }

        // GET /admin/create
        public IActionResult Create()
        {
            return View(new MerchantFormVm());
        }

        // POST /admin/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MerchantFormVm form)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                return View(form);
            }

            try
            {
                await _merchantService.CreateAsync(form);
                TempData["Success"] = "Merchant created successfully.";
                return RedirectToAction(nameof(Merchants));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Error creating merchant: {errorMessage}");
                return View(form);
            }
        }

        // GET /admin/detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var merchant = await _merchantService.GetByIdAsync(id);
            if (merchant == null) return NotFound();

            var activeCount = _merchantService.GetActiveGuestCount(merchant);
            var portalLink = $"{Request.Scheme}://{Request.Host}/portal/{merchant.PortalToken}";

            var vm = new MerchantDetailVm
            {
                Merchant = merchant,
                Invitations = merchant.Invitations
                    .Where(i => i.Status != InvitationStatus.Removed)
                    .ToList(),
                ActiveCount = activeCount,
                RemainingSlots = merchant.MaxInvites - activeCount,
                PortalLink = portalLink
            };

            return View(vm);
        }

        // GET /admin/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var merchant = await _merchantService.GetByIdAsync(id);
            if (merchant == null) return NotFound();

            var form = new MerchantFormVm
            {
                MerchantId = merchant.MerchantId,
                Name = merchant.Name,
                ContactEmail = merchant.ContactEmail,
                ContactPhone = merchant.ContactPhone,
                MaxInvites = merchant.MaxInvites
            };

            return View(form);
        }

        // POST /admin/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MerchantFormVm form)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                return View(form);
            }

            try
            {
                await _merchantService.UpdateAsync(form);
                TempData["Success"] = "Merchant updated successfully.";
                return RedirectToAction(nameof(Detail), new { id = form.MerchantId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating merchant: {ex.Message}");
                return View(form);
            }
        }

        // POST /admin/toggle/5
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            await _merchantService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Merchants));
        }

        // GET /admin/auditlogs
        public async Task<IActionResult> AuditLogs()
        {
            var logs = await _db.AuditLogs
                .Include(a => a.Merchant)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return View(logs);
        }
    }
}
