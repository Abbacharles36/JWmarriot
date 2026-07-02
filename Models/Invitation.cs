namespace BuysimTechnology.Models
{
    public enum InvitationStatus
    {
        Pending,
        CheckedIn,
        Removed
    }

    public class Invitation
    {
        public int InvitationId { get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;

        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string GuestIdNumber { get; set; } = string.Empty;

        // Unique ticket number — e.g. BSM-00042
        public string TicketNumber { get; set; } = string.Empty;

        // QR code stored as Base64 PNG string
        public string QrCodeBase64 { get; set; } = string.Empty;

        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CheckedInAt { get; set; }
    }
}
