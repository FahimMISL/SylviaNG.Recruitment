import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-candidate-notifications',
  standalone: true,
  template: `
    <div class="page-header">
      <div>
        <h1 class="page-title">My Notifications</h1>
        <p class="page-subtitle">Stay updated on your applications</p>
      </div>
      @if (notifications().length > 0) {
        <button class="btn btn-outline" (click)="markAllRead()">Mark all read</button>
      }
    </div>

    @if (loading()) {
      <div class="loading-state">Loading notifications...</div>
    } @else if (notifications().length === 0) {
      <div class="empty-state">
        <div class="empty-icon">🔔</div>
        <h3>No notifications</h3>
        <p>You'll be notified when there are updates about your applications.</p>
      </div>
    } @else {
      <div class="notif-list">
        @for (n of notifications(); track n.userNotificationId) {
          <div class="notif-card" [class.unread]="!n.isRead" (click)="markRead(n)">
            <div class="notif-icon">{{ getIcon(n.notificationType) }}</div>
            <div class="notif-content">
              <h4 class="notif-title">{{ n.title }}</h4>
              <p class="notif-message">{{ n.message }}</p>
              <span class="notif-time">{{ formatTime(n.createdAt) }}</span>
            </div>
            @if (!n.isRead) {
              <div class="unread-dot"></div>
            }
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .empty-state { text-align: center; padding: 3rem; }
    .empty-icon { font-size: 3rem; margin-bottom: 1rem; }
    .empty-state h3 { margin: 0 0 0.5rem; }
    .empty-state p { color: var(--text-secondary, #6b7280); }
    .notif-list { display: flex; flex-direction: column; gap: 0.5rem; }
    .notif-card { display: flex; align-items: flex-start; gap: 0.75rem; background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 8px; padding: 1rem; cursor: pointer; transition: background 0.15s; }
    .notif-card:hover { background: var(--bg-secondary, #f9fafb); }
    .notif-card.unread { background: #eff6ff; border-color: #bfdbfe; }
    .notif-icon { font-size: 1.25rem; flex-shrink: 0; margin-top: 2px; }
    .notif-content { flex: 1; min-width: 0; }
    .notif-title { font-size: 0.875rem; font-weight: 600; margin: 0 0 0.25rem; color: var(--text-primary, #111827); }
    .notif-message { font-size: 0.813rem; color: var(--text-secondary, #6b7280); margin: 0 0 0.25rem; }
    .notif-time { font-size: 0.75rem; color: var(--text-tertiary, #9ca3af); }
    .unread-dot { width: 8px; height: 8px; border-radius: 50%; background: #3b82f6; flex-shrink: 0; margin-top: 6px; }
  `],
})
export class CandidateNotificationsComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  notifications = signal<any[]>([]);
  loading = signal(false);

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<any>(`${this.baseUrl}/user-notifications?limit=50`).subscribe({
      next: (res) => {
        this.notifications.set(res.content ?? res ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  markRead(n: any): void {
    if (n.isRead) return;
    this.http.put(`${this.baseUrl}/user-notifications/${n.userNotificationId}/read`, {}).subscribe({
      next: () => {
        n.isRead = true;
        this.notifications.set([...this.notifications()]);
      },
    });
  }

  markAllRead(): void {
    this.http.put(`${this.baseUrl}/user-notifications/read-all`, {}).subscribe({
      next: () => {
        const updated = this.notifications().map(n => ({ ...n, isRead: true }));
        this.notifications.set(updated);
      },
    });
  }

  getIcon(type: number): string {
    return ['ℹ️', '✅', '⚠️', '🔔', '📧'][type] ?? '🔔';
  }

  formatTime(dateStr: string): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - d.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    const diffDays = Math.floor(diffHours / 24);
    if (diffDays < 7) return `${diffDays}d ago`;
    return d.toLocaleDateString();
  }
}
