using System.ComponentModel.DataAnnotations;

namespace BuysimTechnology.Models.ViewModels
{
    public class MerchantPortalVm
    {
        public Merchant Merchant { get; set; } = null!;
        public List<Invitation> Invitations { get; set; } = new();
        public int ActiveCount { get; set; }
        public int RemainingSlots { get; set; }
        public GuestFormVm GuestForm { get; set; } = new();
    }

    public class GuestFormVm
    {
        public int InvitationId { get; set; }
        public int MerchantId { get; set; }

        [Required(ErrorMessage = "Guest name is required")]
        [Display(Name = "Full Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone Number")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID number is required")]
        [Display(Name = "National ID / Passport Number")]
        public string GuestIdNumber { get; set; } = string.Empty;
    }
}
