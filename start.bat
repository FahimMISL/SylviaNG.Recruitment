@echo off
title SylviaNG.Recruitment - Startup
echo ============================================
echo   SylviaNG.Recruitment - Starting All Services
echo ============================================
echo.

cd /d "%~dp0"

:: Step 1: Docker services
echo [1/5] Starting Docker services (PostgreSQL, Keycloak, Kafka)...
docker compose up -d postgres keycloak kafka
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker Compose failed. Make sure Docker Desktop is running.
    pause
    exit /b 1
)
echo       Docker containers started.
echo.

:: Step 2: Wait for PostgreSQL
echo [2/5] Waiting for PostgreSQL to be ready...
:wait_pg
docker exec recruitment-postgres pg_isready -U postgres >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    timeout /t 2 /nobreak >nul
    goto wait_pg
)
echo       PostgreSQL is ready.
echo.

:: Step 3: Wait for Keycloak
echo [3/5] Waiting for Keycloak to be ready (this may take 30-60 seconds)...
:wait_kc
docker exec recruitment-keycloak bash -c "exec 3<>/dev/tcp/localhost/8080 && echo OK" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    timeout /t 5 /nobreak >nul
    goto wait_kc
)
echo       Keycloak is ready.
echo.

:: Step 4: Wait for Kafka
echo [4/5] Waiting for Kafka to be ready...
:wait_kafka
docker exec recruitment-kafka /opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    timeout /t 5 /nobreak >nul
    goto wait_kafka
)
echo       Kafka is ready.
echo.

:: Step 5: Start Backend and Frontend
echo [5/5] Starting Backend (.NET) and Frontend (Angular)...
echo.
start "Backend - localhost:5208" cmd /k "cd /d %~dp0SylviaNG.Recruitment && dotnet run --urls http://localhost:5208"
start "Frontend - localhost:4200" cmd /k "cd /d %~dp0recruitment-ui && npx ng serve --port 4200"

echo ============================================
echo   All services starting!
echo.
echo   Docker:    PostgreSQL :5432, Keycloak :8082, Kafka :9899
echo   Backend:   http://localhost:5208
echo   Frontend:  http://localhost:4200
echo.
echo   Waiting 20 seconds for backend/frontend to compile...
echo ============================================
echo.

timeout /t 20 /nobreak >nul
start http://localhost:4200

pause
