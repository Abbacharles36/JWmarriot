using BuysimTechnology.Data;
using BuysimTechnology.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ── Services ──────────────────────────────────────────────────────────
builder.Services.AddScoped<MerchantService>();
builder.Services.AddScoped<InvitationService>();
builder.Services.AddScoped<GateService>();
builder.Services.AddScoped<QrCodeService>();
builder.Services.AddScoped<SecureTicketService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Admin route
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });

// Gate route
app.MapControllerRoute(
    name: "gate",
    pattern: "gate/{action=Index}",
    defaults: new { controller = "Gate" });

// Merchant portal — unique token link
app.MapControllerRoute(
    name: "portal",
    pattern: "portal/{token}",
    defaults: new { controller = "MerchantPortal", action = "Index" });

// Default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
