# VERSION 1 ROLLBACK SCRIPT (PowerShell)
# This script restores all VERSION 1 files from backup
# Usage: .\ROLLBACK_VERSION_1.ps1

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "  VERSION 1 ROLLBACK SCRIPT" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

$PROJECT_PATH = "C:\Users\abbac\source\repos\JWmarriot"
$BACKUP_PATH = "$PROJECT_PATH\VERSION_1_BACKUP"

Write-Host "Restoring VERSION 1 files..." -ForegroundColor Yellow
Write-Host ""

# Restore Admin views
Write-Host "Restoring Admin/Create.cshtml..." -ForegroundColor Green
Copy-Item "$BACKUP_PATH\Create.cshtml" "$PROJECT_PATH\Views\Admin\Create.cshtml" -Force

Write-Host "Restoring Admin/Edit.cshtml..." -ForegroundColor Green
Copy-Item "$BACKUP_PATH\Edit.cshtml" "$PROJECT_PATH\Views\Admin\Edit.cshtml" -Force

# Restore MerchantPortal view
Write-Host "Restoring MerchantPortal/Index.cshtml..." -ForegroundColor Green
Copy-Item "$BACKUP_PATH\Index.cshtml" "$PROJECT_PATH\Views\MerchantPortal\Index.cshtml" -Force

# Restore Controller
Write-Host "Restoring MerchantPortalController.cs..." -ForegroundColor Green
Copy-Item "$BACKUP_PATH\MerchantPortalController.cs" "$PROJECT_PATH\Controllers\MerchantPortalController.cs" -Force

Write-Host ""
Write-Host "================================" -ForegroundColor Green
Write-Host "  ✓ ROLLBACK COMPLETE" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""
Write-Host "All VERSION 1 files have been restored." -ForegroundColor Yellow
Write-Host "You can now rebuild the project." -ForegroundColor Yellow
Write-Host ""
