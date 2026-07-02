using BuysimTechnology.Models.ViewModels;
using BuysimTechnology.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuysimTechnology.Controllers
{
    public class GateController : Controller
    {
        private readonly GateService _gateService;

        public GateController(GateService gateService)
        {
            _gateService = gateService;
        }

        // GET /gate
        public IActionResult Index()
        {
            return View(new GateVm());
        }

        // POST /gate/validate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(GateVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.TicketInput))
            {
                ModelState.AddModelError("", "Please enter a ticket number.");
                return View("Index", vm);
            }

            vm.Result = await _gateService.ValidateTicketAsync(vm.TicketInput);
            return View("Index", vm);
        }
    }
}
