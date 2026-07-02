namespace BuysimTechnology.Models.ViewModels
{
    public class MerchantDetailVm
    {
        public Merchant Merchant { get; set; } = null!;
        public List<Invitation> Invitations { get; set; } = new();
        public int ActiveCount { get; set; }
        public int RemainingSlots { get; set; }
        public string PortalLink { get; set; } = string.Empty;
    }
}
