using BuysimTechnology.Data;
using Microsoft.EntityFrameworkCore;

// Quick diagnostic to verify database and connections
public class DiagnosticService
{
    private readonly AppDbContext _db;

    public DiagnosticService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool Success, string Message)> CheckDatabaseAsync()
    {
        try
        {
            // Check if database can be connected
            var canConnect = await _db.Database.CanConnectAsync();
            if (!canConnect)
                return (false, "Cannot connect to database");

            // Check table counts
            var merchantCount = await _db.Merchants.CountAsync();
            var invitationCount = await _db.Invitations.CountAsync();
            var auditCount = await _db.AuditLogs.CountAsync();

            return (true, $"✓ Database OK | Merchants: {merchantCount}, Invitations: {invitationCount}, Audit Logs: {auditCount}");
        }
        catch (Exception ex)
        {
            return (false, $"Database Error: {ex.Message}");
        }
    }
}
