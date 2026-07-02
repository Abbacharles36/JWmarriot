namespace BuysimTechnology.Models
{
    public class Merchant
    {
        public int MerchantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        // Unique token used to generate the merchant's private link
        // e.g. /portal/abc123xyz
        public string PortalToken { get; set; } = string.Empty;

        // Admin sets this per merchant — between 1 and 5
        public int MaxInvites { get; set; } = 5;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
