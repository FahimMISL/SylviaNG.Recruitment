@echo off
title SylviaNG.Recruitment - Reset Database
echo ============================================
echo   SylviaNG.Recruitment - RESET DATABASE
echo ============================================
echo.
echo   WARNING: This will DELETE all data except
echo   the admin user and re-run migrations.
echo.
echo   - All candidates will be removed
echo   - All HR users will be removed
echo   - All job postings, applications, payments etc. will be cleared
echo   - Only Admin (Abir Hasan) will remain
echo.
set /p confirm="Type YES to confirm: "
if /i not "%confirm%"=="YES" (
    echo Cancelled.
    pause
    exit /b 0
)
echo.

:: Check Docker is running
docker exec recruitment-postgres pg_isready -U postgres >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: PostgreSQL container is not running. Start Docker first.
    pause
    exit /b 1
)

echo [1/4] Stopping backend if running...
taskkill /F /IM SylviaNG.Recruitment.exe >nul 2>&1
timeout /t 2 /nobreak >nul
echo       Done.
echo.

echo [2/4] Dropping and recreating the database...
docker exec recruitment-postgres psql -U postgres -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'recruitment_db' AND pid <> pg_backend_pid();" >nul 2>&1
docker exec recruitment-postgres psql -U postgres -c "DROP DATABASE IF EXISTS recruitment_db;"
docker exec recruitment-postgres psql -U postgres -c "CREATE DATABASE recruitment_db;"
if %errorlevel% neq 0 (
    echo ERROR: Failed to recreate database.
    pause
    exit /b 1
)
echo       Database recreated.
echo.

echo [3/4] Running EF Core migrations...
cd /d "%~dp0SylviaNG.Recruitment"
dotnet run --launch-profile http -- --migrate-only >nul 2>&1
:: Migrations run automatically on startup, so just start and let it migrate
echo       Starting backend to apply migrations...
start /b "" cmd /c "dotnet run --launch-profile http > nul 2>&1"

:: Wait for backend to be ready (migrations applied)
echo       Waiting for backend to be ready...
:wait_api
timeout /t 3 /nobreak >nul
curl -s -o nul -w "%%{http_code}" http://localhost:5208/recruitment/health 2>nul | findstr "200" >nul 2>&1
if %errorlevel% neq 0 goto wait_api
echo       Backend is ready, migrations applied.
echo.

echo [4/4] Seeding admin user...
docker exec recruitment-postgres psql -U postgres -d recruitment_db -c "INSERT INTO \"Users\" (\"KeycloakUserId\", \"FullName\", \"Email\", \"IsActive\", \"TenantId\", \"Status\") VALUES ('dcfe224b-1cb8-4d20-ac59-85fa47d526b4', 'Abir Hasan', 'abirha3896@gmail.com', true, 'default_tenant', 0) ON CONFLICT (\"KeycloakUserId\") DO NOTHING;"
if %errorlevel% neq 0 (
    echo WARNING: Admin user insert may have failed. Check manually.
) else (
    echo       Admin user seeded.
)
echo.

:: Stop the backend (user will restart with start.bat)
taskkill /F /IM SylviaNG.Recruitment.exe >nul 2>&1

echo ============================================
echo   Database reset complete!
echo.
echo   Admin: Abir Hasan (abirha3896@gmail.com)
echo   Role:  Admin (managed in Keycloak)
echo.
echo   Run start.bat to start the project.
echo ============================================
echo.
pause
