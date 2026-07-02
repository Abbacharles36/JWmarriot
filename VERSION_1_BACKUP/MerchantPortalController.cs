using BuysimTechnology.Models;
using BuysimTechnology.Models.ViewModels;
using BuysimTechnology.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuysimTechnology.Controllers
{
    public class MerchantPortalController : Controller
    {
        private readonly MerchantService _merchantService;
        private readonly InvitationService _invitationService;

        public MerchantPortalController(MerchantService merchantService,
                                        InvitationService invitationService)
        {
            _merchantService = merchantService;
            _invitationService = invitationService;
        }

        // GET /portal/{token}
        // This is the unique link each merchant receives
        public async Task<IActionResult> Index(string token)
        {
            var merchant = await _merchantService.GetByTokenAsync(token);
            if (merchant == null)
                return View("InvalidLink");

            var activeCount = _merchantService.GetActiveGuestCount(merchant);

            var vm = new MerchantPortalVm
            {
                Merchant = merchant,
                Invitations = merchant.Invitations
                    .Where(i => i.Status != InvitationStatus.Removed)
                    .ToList(),
                ActiveCount = activeCount,
                RemainingSlots = merchant.MaxInvites - activeCount,
                GuestForm = new GuestFormVm { MerchantId = merchant.MerchantId }
            };

            return View(vm);
        }

        // POST /portal/addguest?token=abc123
        [HttpPost("/portal/addguest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGuest(GuestFormVm form, string token)
        {
            // Validate that merchant exists and token matches
            var merchant = await _merchantService.GetByTokenAsync(token);
            if (merchant == null)
            {
                TempData["Error"] = "Invalid or expired merchant link.";
                return View("InvalidLink");
            }

            // Set MerchantId from the token lookup if not already set
            if (form.MerchantId == 0)
                form.MerchantId = merchant.MerchantId;

            if (!ModelState.IsValid)
            {
                // Return to form with validation errors displayed
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = $"Validation failed: {errors}";

                var activeCount = _merchantService.GetActiveGuestCount(merchant);
                var vm = new MerchantPortalVm
                {
                    Merchant = merchant,
                    Invitations = merchant.Invitations
                        .Where(i => i.Status != InvitationStatus.Removed).ToList(),
                    ActiveCount = activeCount,
                    RemainingSlots = merchant.MaxInvites - activeCount,
                    GuestForm = form
                };
                return View("Index", vm);
            }

            // Ensure MerchantId is set correctly before saving
            if (form.MerchantId == 0)
                form.MerchantId = merchant.MerchantId;

            try
            {
                var (success, error) = await _invitationService.AddGuestAsync(form);

                if (!success)
                    TempData["Error"] = error;
                else
                    TempData["Success"] = "Guest added successfully! Their ticket has been generated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { token });
        }

        // POST /portal/removeguest?token=abc123
        [HttpPost("/portal/removeguest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveGuest(int invitationId, string token)
        {
            var (success, error) = await _invitationService.RemoveGuestAsync(invitationId);

            if (!success)
                TempData["Error"] = error;
            else
                TempData["Success"] = "Guest removed. The slot is now available.";

            return RedirectToAction(nameof(Index), new { token });
        }
    }
}
