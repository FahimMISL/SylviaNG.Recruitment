import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from './sidebar.component';
import { HeaderComponent } from './header.component';
import { ToastContainerComponent } from './toast-container.component';
import { KeycloakService } from '../services/keycloak.service';
import { ApiService } from '../services/api.service';
import { NotificationService } from '../services/notification.service';
import { ProfilePhotoService } from '../services/profile-photo.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, HeaderComponent, ToastContainerComponent],
  template: `
    <div class="app-shell">
      <app-sidebar [collapsed]="sidebarCollapsed()" />
      <div class="app-main" [class.sidebar-collapsed]="sidebarCollapsed()">
        <app-header (toggleSidebar)="sidebarCollapsed.set(!sidebarCollapsed())" />
        <main class="app-content">
          <router-outlet />
        </main>
      </div>
    </div>
    <app-toast-container />
  `,
})
export class LayoutComponent implements OnInit, OnDestroy {
  private keycloak = inject(KeycloakService);
  private api = inject(ApiService);
  private notificationService = inject(NotificationService);
  private profilePhotoService = inject(ProfilePhotoService);

  sidebarCollapsed = signal(false);

  ngOnInit(): void {
    this.autoProvisionCandidateIfNeeded();
    this.notificationService.startPolling();
    this.profilePhotoService.loadMyPhoto();
  }

  ngOnDestroy(): void {
    this.notificationService.stopPolling();
  }

  private autoProvisionCandidateIfNeeded(): void {
    const user = this.keycloak.currentUser();
    if (!user) return;

    // Only auto-provision for users with the Candidate role
    const isCandidate = user.roles.some(
      (r) => r.toLowerCase() === 'candidate'
    );
    if (!isCandidate) return;

    // Already provisioned in this session
    if (user.candidateId) return;

    this.api
      .create<number>('candidate-self/auto-provision', {
        keycloakUserId: user.keycloakUserId,
        fullName: user.fullName,
        email: user.email,
      })
      .subscribe({
        next: (candidateId) => {
          this.keycloak.updateCandidateId(candidateId);
        },
        error: (err) => {
          console.error('Failed to auto-provision candidate profile', err);
        },
      });
  }
}
