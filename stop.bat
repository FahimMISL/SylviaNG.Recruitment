@echo off
title SylviaNG.Recruitment - Shutdown
echo ============================================
echo   SylviaNG.Recruitment - Stopping All Services
echo ============================================
echo.

cd /d "%~dp0"

echo [1/3] Stopping Backend...
taskkill /F /IM SylviaNG.Recruitment.exe >nul 2>&1
echo       Backend stopped.

echo [2/3] Stopping Frontend...
taskkill /F /FI "WINDOWTITLE eq Frontend*" >nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :4200 ^| findstr LISTENING 2^>nul') do taskkill /F /PID %%a >nul 2>&1
echo       Frontend stopped.

echo [3/3] Stopping Docker services...
docker compose down
echo       Docker services stopped.

echo.
echo   All services stopped.
echo.
pause
