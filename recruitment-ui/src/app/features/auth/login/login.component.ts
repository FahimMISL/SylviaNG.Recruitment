import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <div class="login-page">
      <div class="login-brand">
        <h1 class="login-brand-title">Smart Recruitment Platform</h1>
        <p class="login-brand-subtitle">
          AI-Powered Hiring — powered by Millennium Information Solution Ltd.
        </p>
      </div>
      <div class="login-form-panel">
        <div class="login-form" style="text-align: center;">
          <h2 class="login-form-title">Welcome</h2>
          <p class="login-form-subtitle">Sign in or register to access the recruitment platform</p>

          <button class="btn btn-primary btn-lg w-100" (click)="onLogin()" style="margin-top: var(--space-6);">
            Sign In with Keycloak
          </button>

          <button class="btn btn-outline btn-lg w-100" (click)="onRegister()" style="margin-top: var(--space-3);">
            Register as Candidate
          </button>
        </div>
      </div>
    </div>
  `,
})
export class LoginComponent implements OnInit {
  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onLogin(): void {
    this.auth.login();
  }

  onRegister(): void {
    this.auth.register();
  }
}
