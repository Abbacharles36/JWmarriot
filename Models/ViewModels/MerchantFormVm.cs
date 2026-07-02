using System.ComponentModel.DataAnnotations;

namespace BuysimTechnology.Models.ViewModels
{
    public class MerchantFormVm
    {
        public int MerchantId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Guest limit must be between 1 and 5")]
        [Display(Name = "Maximum Guests Allowed")]
        public int MaxInvites { get; set; } = 5;
    }
}
