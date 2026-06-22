import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import Keycloak from 'keycloak-js';
import { AuthUser } from './auth.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class KeycloakService {
  private kc!: Keycloak;
  private _ready = signal(false);
  private _authenticated = signal(false);
  private _user = signal<AuthUser | null>(null);

  ready = this._ready.asReadonly();
  currentUser = this._user.asReadonly();
  isAuthenticated = this._authenticated.asReadonly();

  constructor(private router: Router) {}

  async init(): Promise<boolean> {
    this.kc = new Keycloak({
      url: environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId,
    });

    try {
      const authenticated = await this.kc.init({
        onLoad: 'check-sso',
        checkLoginIframe: false,
      });

      this._authenticated.set(authenticated);

      if (authenticated) {
        this.buildUserFromToken();
        this.loadUserProfileAsync();
        await this.syncProfileIfNeeded();
      }

      setInterval(() => {
        this.kc.updateToken(60).catch(() => this.logout());
      }, 30000);

      this._ready.set(true);
      return authenticated;
    } catch (err) {
      console.error('Keycloak init failed', err);
      this._ready.set(true);
      return false;
    }
  }

  private async syncProfileIfNeeded(): Promise<void> {
    const roles = this.getRoles();
    if (roles.length > 0) return;

    try {
      const res = await fetch(`${environment.apiUrl}/auth/sync-profile`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.kc.token}`,
          'Content-Type': 'application/json',
        },
      });
      if (!res.ok) return;

      await this.kc.updateToken(-1);
      this.buildUserFromToken();
    } catch (err) {
      console.error('Profile sync failed', err);
    }
  }

  login(): void {
    this.kc.login({ redirectUri: window.location.origin + '/dashboard' });
  }

  register(): void {
    this.kc.register({ redirectUri: window.location.origin + '/dashboard' });
  }

  logout(): void {
    this._authenticated.set(false);
    this._user.set(null);
    this.kc.logout({ redirectUri: window.location.origin + '/login' });
  }

  getToken(): string | undefined {
    return this.kc?.token;
  }

  accountAction(action: 'UPDATE_PASSWORD' | 'UPDATE_PROFILE' | 'UPDATE_EMAIL'): void {
    this.kc.login({
      action,
      redirectUri: window.location.origin + '/profile',
    });
  }

  hasRole(role: string): boolean {
    return this.kc?.hasRealmRole(role) ?? false;
  }

  getRoles(): string[] {
    return this.kc?.realmAccess?.roles?.filter(
      (r) => !r.startsWith('default-roles-') && r !== 'offline_access' && r !== 'uma_authorization'
    ) ?? [];
  }

  updateCandidateId(candidateId: number): void {
    const current = this._user();
    if (current) {
      this._user.set({ ...current, candidateId });
    }
  }

  private buildUserFromToken(): void {
    const parsed = this.kc.tokenParsed;
    if (!parsed) return;
    this._user.set({
      userId: 0,
      keycloakUserId: parsed.sub ?? '',
      username: parsed['preferred_username'] ?? '',
      fullName: parsed['name'] ?? parsed['preferred_username'] ?? '',
      email: parsed['email'] ?? '',
      roles: this.getRoles(),
      tenantId: 'default_tenant',
    });
  }

  private loadUserProfileAsync(): void {
    this.kc.loadUserProfile().then((profile) => {
      this._user.set({
        userId: 0,
        keycloakUserId: profile.id ?? '',
        username: profile.username ?? '',
        fullName: `${profile.firstName ?? ''} ${profile.lastName ?? ''}`.trim(),
        email: profile.email ?? '',
        roles: this.getRoles(),
        tenantId: 'default_tenant',
      });
    }).catch(() => {});
  }
}
