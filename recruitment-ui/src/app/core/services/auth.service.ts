import { Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from './keycloak.service';

export interface AuthUser {
  userId: number;
  keycloakUserId: string;
  username: string;
  fullName: string;
  email: string;
  roles: string[];
  tenantId: string;
  candidateId?: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  currentUser = computed(() => this.kc.currentUser());
  isAuthenticated = computed(() => this.kc.isAuthenticated());

  constructor(private kc: KeycloakService, private router: Router) {}

  async init(): Promise<boolean> {
    return this.kc.init();
  }

  login(): void {
    this.kc.login();
  }

  register(): void {
    this.kc.register();
  }

  logout(): void {
    this.kc.logout();
  }

  getToken(): string | null {
    return this.kc.getToken() ?? null;
  }

  hasRole(role: string): boolean {
    return this.kc.hasRole(role);
  }

  accountAction(action: 'UPDATE_PASSWORD' | 'UPDATE_PROFILE' | 'UPDATE_EMAIL'): void {
    this.kc.accountAction(action);
  }
}
