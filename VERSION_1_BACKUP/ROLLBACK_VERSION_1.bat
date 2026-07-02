@echo off
REM VERSION 1 ROLLBACK SCRIPT
REM This script restores all VERSION 1 files from backup
REM Usage: Run this script to rollback to VERSION 1

echo.
echo ================================
echo  VERSION 1 ROLLBACK SCRIPT
echo ================================
echo.

SET PROJECT_PATH=C:\Users\abbac\source\repos\JWmarriot
SET BACKUP_PATH=%PROJECT_PATH%\VERSION_1_BACKUP

echo Restoring VERSION 1 files...
echo.

REM Restore Admin views
echo Restoring Admin/Create.cshtml...
copy "%BACKUP_PATH%\Create.cshtml" "%PROJECT_PATH%\Views\Admin\Create.cshtml" /Y

echo Restoring Admin/Edit.cshtml...
copy "%BACKUP_PATH%\Edit.cshtml" "%PROJECT_PATH%\Views\Admin\Edit.cshtml" /Y

REM Restore MerchantPortal view
echo Restoring MerchantPortal/Index.cshtml...
copy "%BACKUP_PATH%\Index.cshtml" "%PROJECT_PATH%\Views\MerchantPortal\Index.cshtml" /Y

REM Restore Controller
echo Restoring MerchantPortalController.cs...
copy "%BACKUP_PATH%\MerchantPortalController.cs" "%PROJECT_PATH%\Controllers\MerchantPortalController.cs" /Y

echo.
echo ================================
echo  ✓ ROLLBACK COMPLETE
echo ================================
echo.
echo All VERSION 1 files have been restored.
echo You can now rebuild the project.
echo.
pause
