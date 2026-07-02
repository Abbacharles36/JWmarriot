namespace BuysimTechnology.Models
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string PerformedBy { get; set; } = string.Empty;
    }
}
