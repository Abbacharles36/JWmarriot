namespace BuysimTechnology.Models.ViewModels
{
    public class GateVm
    {
        public string TicketInput { get; set; } = string.Empty;
        public GateResultVm? Result { get; set; }
    }

    public class GateResultVm
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
