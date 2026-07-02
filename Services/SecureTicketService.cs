using System.Security.Cryptography;
using System.Text;
using BuysimTechnology.Data;
using BuysimTechnology.Models;
using Microsoft.EntityFrameworkCore;

namespace BuysimTechnology.Services
{
    /// <summary>
    /// Generates secure, non-sequential ticket numbers to prevent forgery
    /// </summary>
    public class SecureTicketService
    {
        private readonly AppDbContext _db;
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int RANDOM_PART_LENGTH = 12; // Random alphanumeric characters

        public SecureTicketService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Generates a unique, non-sequential, cryptographically secure ticket number
        /// Format: BSM-XXXXXXXXXX (BSM prefix + 12 random alphanumeric characters)
        /// Example: BSM-K7X9M2P4Q1R8
        /// </summary>
        public async Task<string> GenerateSecureTicketAsync()
        {
            string ticketNumber;
            bool isUnique = false;
            int attempts = 0;
            const int maxAttempts = 10; // Prevent infinite loops

            // Keep generating until we get a unique ticket
            while (!isUnique && attempts < maxAttempts)
            {
                ticketNumber = GenerateRandomTicket();

                // Check if ticket already exists in database
                var exists = await _db.Invitations
                    .AnyAsync(i => i.TicketNumber == ticketNumber);

                if (!exists)
                {
                    isUnique = true;
                    return ticketNumber;
                }

                attempts++;
            }

            // Fallback (extremely unlikely)
            throw new InvalidOperationException("Failed to generate unique ticket after multiple attempts");
        }

        /// <summary>
        /// Generates a random secure ticket number
        /// Format: BSM-XXXXXXXXXX
        /// </summary>
        private string GenerateRandomTicket()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[RANDOM_PART_LENGTH];
                rng.GetBytes(randomBytes);

                var sb = new StringBuilder();
                foreach (byte b in randomBytes)
                {
                    // Convert random byte to index in ALPHABET
                    sb.Append(ALPHABET[b % ALPHABET.Length]);
                }

                return $"BSM-{sb.ToString()}";
            }
        }

        /// <summary>
        /// Validates a ticket number format
        /// </summary>
        public bool ValidateTicketFormat(string ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket))
                return false;

            if (!ticket.StartsWith("BSM-"))
                return false;

            var parts = ticket.Split('-');
            if (parts.Length != 2)
                return false;

            var randomPart = parts[1];
            if (randomPart.Length != RANDOM_PART_LENGTH)
                return false;

            // Check if all characters are valid alphanumeric
            return randomPart.All(c => ALPHABET.Contains(c));
        }

        /// <summary>
        /// Gets ticket statistics for audit/verification
        /// </summary>
        public async Task<(int Total, int Pending, int CheckedIn, int Removed)> GetTicketStatsAsync()
        {
            var total = await _db.Invitations.CountAsync();
            var pending = await _db.Invitations.CountAsync(i => i.Status == InvitationStatus.Pending);
            var checkedIn = await _db.Invitations.CountAsync(i => i.Status == InvitationStatus.CheckedIn);
            var removed = await _db.Invitations.CountAsync(i => i.Status == InvitationStatus.Removed);

            return (total, pending, checkedIn, removed);
        }
    }
}
