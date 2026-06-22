#!/bin/bash
# Keycloak realm setup script for Smart Recruitment Platform
# Run after Keycloak is started on localhost:8082

KC_URL="http://localhost:8082"
ADMIN_USER="admin"
ADMIN_PASS="admin"
REALM="sylviang"
CLIENT_ID="sylviang-api"
CLIENT_SECRET="recruitment-client-secret-2026"
FRONTEND_CLIENT="sylviang-frontend"

echo "=== Waiting for Keycloak to be ready ==="
until curl -sf "$KC_URL/realms/master" > /dev/null 2>&1; do
  echo "  Keycloak not ready, waiting..."
  sleep 3
done
echo "  Keycloak is ready!"

echo "=== Getting admin token ==="
TOKEN=$(curl -s -X POST "$KC_URL/realms/master/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=$ADMIN_USER&password=$ADMIN_PASS&grant_type=password&client_id=admin-cli" \
  | python -c "import sys,json; print(json.load(sys.stdin)['access_token'])" 2>/dev/null)

if [ -z "$TOKEN" ]; then
  echo "ERROR: Failed to get admin token"
  exit 1
fi
echo "  Got admin token"

echo "=== Creating realm: $REALM ==="
curl -sf -X POST "$KC_URL/admin/realms" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "realm": "'"$REALM"'",
    "enabled": true,
    "displayName": "Smart Recruitment Platform",
    "registrationAllowed": true,
    "registrationEmailAsUsername": false,
    "loginWithEmailAllowed": true,
    "duplicateEmailsAllowed": false,
    "resetPasswordAllowed": true,
    "editUsernameAllowed": false,
    "bruteForceProtected": true,
    "failureFactor": 3,
    "maxDeltaTimeSeconds": 900,
    "maxFailureWaitSeconds": 900,
    "minimumQuickLoginWaitSeconds": 60,
    "waitIncrementSeconds": 300,
    "quickLoginCheckMilliSeconds": 1000,
    "permanentLockout": false,
    "accessTokenLifespan": 28800,
    "ssoSessionIdleTimeout": 86400
  }' && echo "  Realm created" || echo "  Realm may already exist"

echo "=== Creating backend client: $CLIENT_ID ==="
curl -sf -X POST "$KC_URL/admin/realms/$REALM/clients" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "'"$CLIENT_ID"'",
    "name": "Recruitment API",
    "enabled": true,
    "clientAuthenticatorType": "client-secret",
    "secret": "'"$CLIENT_SECRET"'",
    "bearerOnly": true,
    "standardFlowEnabled": false,
    "directAccessGrantsEnabled": true,
    "serviceAccountsEnabled": false,
    "publicClient": false
  }' && echo "  Backend client created" || echo "  Backend client may already exist"

echo "=== Creating frontend client: $FRONTEND_CLIENT ==="
curl -sf -X POST "$KC_URL/admin/realms/$REALM/clients" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "'"$FRONTEND_CLIENT"'",
    "name": "Recruitment Frontend",
    "enabled": true,
    "publicClient": true,
    "standardFlowEnabled": true,
    "directAccessGrantsEnabled": true,
    "implicitFlowEnabled": false,
    "rootUrl": "http://localhost:4200",
    "redirectUris": ["http://localhost:4200/*"],
    "webOrigins": ["http://localhost:4200"],
    "adminUrl": "http://localhost:4200"
  }' && echo "  Frontend client created" || echo "  Frontend client may already exist"

echo "=== Creating realm roles ==="
for ROLE in Admin HR Candidate; do
  curl -sf -X POST "$KC_URL/admin/realms/$REALM/roles" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d '{"name": "'"$ROLE"'"}' && echo "  Role $ROLE created" || echo "  Role $ROLE may already exist"
done

echo "=== Creating default users ==="

create_user() {
  local USERNAME=$1
  local PASSWORD=$2
  local FIRSTNAME=$3
  local LASTNAME=$4
  local EMAIL=$5
  local ROLE=$6

  # Create user
  curl -sf -X POST "$KC_URL/admin/realms/$REALM/users" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d '{
      "username": "'"$USERNAME"'",
      "email": "'"$EMAIL"'",
      "firstName": "'"$FIRSTNAME"'",
      "lastName": "'"$LASTNAME"'",
      "enabled": true,
      "emailVerified": true,
      "credentials": [{"type": "password", "value": "'"$PASSWORD"'", "temporary": true}]
    }' && echo "  User $USERNAME created" || echo "  User $USERNAME may already exist"

  # Get user ID
  USER_ID=$(curl -sf "$KC_URL/admin/realms/$REALM/users?username=$USERNAME" \
    -H "Authorization: Bearer $TOKEN" \
    | python -c "import sys,json; print(json.load(sys.stdin)[0]['id'])" 2>/dev/null)

  if [ -n "$USER_ID" ]; then
    # Get role representation
    ROLE_JSON=$(curl -sf "$KC_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $TOKEN")

    # Assign role
    curl -sf -X POST "$KC_URL/admin/realms/$REALM/users/$USER_ID/role-mappings/realm" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "[$ROLE_JSON]" && echo "  Role $ROLE assigned to $USERNAME" || echo "  Role assignment failed"
  fi
}

create_user "admin" "admin123" "System" "Administrator" "admin@millennium.com" "Admin"
create_user "hrmanager" "hr123" "HR" "Manager" "hr@millennium.com" "HR"

echo ""
echo "=== Setup complete! ==="
echo "Keycloak Admin: $KC_URL (admin/admin)"
echo "Realm: $REALM"
echo "Frontend Client: $FRONTEND_CLIENT (public)"
echo "Backend Client: $CLIENT_ID (secret: $CLIENT_SECRET)"
echo "Users: admin/admin123, hrmanager/hr123"
