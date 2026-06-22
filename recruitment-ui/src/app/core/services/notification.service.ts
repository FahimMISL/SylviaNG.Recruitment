import { Injectable, signal, computed, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface UserNotification {
  userNotificationId: number;
  title: string;
  message: string;
  notificationType: 'Info' | 'Success' | 'Warning' | 'Error';
  actionUrl: string | null;
  isRead: boolean;
  createdAt: string;
  readAt: string | null;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private baseUrl = `${environment.apiUrl}/user-notifications`;
  private pollTimer: ReturnType<typeof setInterval> | null = null;

  notifications = signal<UserNotification[]>([]);
  unreadCount = signal(0);
  hasUnread = computed(() => this.unreadCount() > 0);

  constructor(private http: HttpClient, private auth: AuthService) {}

  startPolling(): void {
    if (this.pollTimer) return;
    this.refresh();
    this.pollTimer = setInterval(() => this.refresh(), 30000);
  }

  stopPolling(): void {
    if (this.pollTimer) {
      clearInterval(this.pollTimer);
      this.pollTimer = null;
    }
  }

  refresh(): void {
    if (!this.auth.isAuthenticated()) return;

    this.http.get<any>(`${this.baseUrl}?limit=20`).subscribe({
      next: (res) => {
        const list = res.content ?? res ?? [];
        this.notifications.set(Array.isArray(list) ? list : []);
      },
      error: () => {},
    });

    this.http.get<any>(`${this.baseUrl}/unread-count`).subscribe({
      next: (res) => {
        const count = res.content ?? res ?? 0;
        this.unreadCount.set(typeof count === 'number' ? count : 0);
      },
      error: () => {},
    });
  }

  markAsRead(id: number): void {
    this.http.put(`${this.baseUrl}/${id}/read`, {}).subscribe({
      next: () => {
        this.notifications.update((list) =>
          list.map((n) => (n.userNotificationId === id ? { ...n, isRead: true } : n))
        );
        this.unreadCount.update((c) => Math.max(0, c - 1));
      },
    });
  }

  markAllAsRead(): void {
    this.http.put(`${this.baseUrl}/read-all`, {}).subscribe({
      next: () => {
        this.notifications.update((list) => list.map((n) => ({ ...n, isRead: true })));
        this.unreadCount.set(0);
      },
    });
  }

  clearAll(): void {
    this.http.delete(`${this.baseUrl}/clear-all`).subscribe({
      next: () => {
        this.notifications.set([]);
        this.unreadCount.set(0);
      },
    });
  }
}
