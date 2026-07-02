namespace BuysimTechnology.Models.ViewModels
{
    public class AdminDashboardVm
    {
        public int TotalMerchants { get; set; }
        public int TotalGuests { get; set; }
        public int TotalCheckedIn { get; set; }
        public int TotalPending { get; set; }
        public List<Merchant> RecentMerchants { get; set; } = new();
    }
}
