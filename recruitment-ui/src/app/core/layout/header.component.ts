import { Component, output, signal, HostListener } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { ProfilePhotoService } from '../services/profile-photo.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink],
  template: `
    <header class="app-header">
      <div class="header-left">
        <button class="header-toggle-btn" (click)="toggleSidebar.emit()">
          &#9776;
        </button>
        <div class="header-search">
          <span>&#128269;</span>
          <input type="text" placeholder="Search anything..." />
        </div>
      </div>
      <div class="header-right">
        <!-- Notification bell -->
        <div class="header-dropdown-wrapper">
          <button class="header-notification" (click)="toggleNotifications($event)">
            &#128276;
            @if (notifications.hasUnread()) {
              <span class="notification-badge">{{ notifications.unreadCount() > 9 ? '9+' : notifications.unreadCount() }}</span>
            }
          </button>
          @if (showNotifications()) {
            <div class="header-dropdown notification-dropdown">
              <div class="notification-dropdown-header">
                <span class="header-dropdown-title">Notifications</span>
                <div class="notification-header-actions">
                  @if (notifications.hasUnread()) {
                    <button class="notification-mark-all" (click)="onMarkAllRead($event)">Mark all read</button>
                  }
                  @if (notifications.notifications().length > 0) {
                    <button class="notification-clear-all" (click)="onClearAll($event)">Clear all</button>
                  }
                </div>
              </div>
              @if (notifications.notifications().length === 0) {
                <div class="notification-empty">No notifications yet</div>
              } @else {
                <div class="notification-list">
                  @for (n of notifications.notifications(); track n.userNotificationId) {
                    <div class="notification-item" [class.unread]="!n.isRead" (click)="onNotificationClick(n, $event)">
                      <div class="notification-item-icon" [class]="'ntype-' + n.notificationType.toLowerCase()">
                        {{ getNotificationIcon(n.notificationType) }}
                      </div>
                      <div class="notification-item-content">
                        <div class="notification-item-title">{{ n.title }}</div>
                        <div class="notification-item-message">{{ n.message }}</div>
                        <div class="notification-item-time">{{ timeAgo(n.createdAt) }}</div>
                      </div>
                    </div>
                  }
                </div>
              }
            </div>
          }
        </div>

        <!-- User avatar & dropdown -->
        <div class="header-dropdown-wrapper">
          <div class="header-avatar" (click)="toggleUserMenu($event)">
            @if (profilePhoto.photoUrl()) {
              <img [src]="profilePhoto.photoUrl()" alt="Avatar" class="header-avatar-img" />
            } @else {
              {{ initials }}
            }
          </div>
          @if (showUserMenu()) {
            <div class="header-dropdown user-dropdown">
              <div class="user-dropdown-info">
                <div class="user-dropdown-name">{{ auth.currentUser()?.fullName ?? 'User' }}</div>
                <div class="user-dropdown-email">{{ auth.currentUser()?.email ?? '' }}</div>
                <div class="user-dropdown-role">{{ primaryRole }}</div>
              </div>
              <div class="header-dropdown-divider"></div>
              <a class="header-dropdown-item" routerLink="/profile" (click)="showUserMenu.set(false)">
                <span>&#9998;</span> {{ isStaffUser ? 'Account Settings' : 'My Profile' }}
              </a>
              <button class="header-dropdown-item logout-item" (click)="onLogout()">
                <span>&#x2BBE;</span> Sign out
              </button>
            </div>
          }
        </div>
      </div>
    </header>
  `,
  styles: [`
    .notification-badge {
      position: absolute;
      top: -4px;
      right: -4px;
      background: var(--color-danger, #ef4444);
      color: white;
      font-size: 10px;
      font-weight: 700;
      min-width: 18px;
      height: 18px;
      border-radius: 9px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0 4px;
      line-height: 1;
    }
    .header-notification {
      position: relative;
    }
    .notification-dropdown {
      width: 360px;
      max-height: 420px;
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }
    .notification-dropdown-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 16px;
      border-bottom: 1px solid var(--color-border, #e5e7eb);
    }
    .notification-mark-all {
      background: none;
      border: none;
      color: var(--color-primary, #3b82f6);
      font-size: 12px;
      cursor: pointer;
      font-weight: 500;
    }
    .notification-header-actions {
      display: flex;
      gap: 12px;
      align-items: center;
    }
    .notification-mark-all:hover, .notification-clear-all:hover {
      text-decoration: underline;
    }
    .notification-clear-all {
      background: none;
      border: none;
      color: var(--color-danger, #ef4444);
      font-size: 12px;
      cursor: pointer;
      font-weight: 500;
    }
    .notification-list {
      overflow-y: auto;
      max-height: 360px;
    }
    .notification-item {
      display: flex;
      gap: 12px;
      padding: 12px 16px;
      cursor: pointer;
      transition: background 0.15s;
      border-bottom: 1px solid var(--color-border, #f3f4f6);
    }
    .notification-item:hover {
      background: var(--color-bg-secondary, #f9fafb);
    }
    .notification-item.unread {
      background: var(--color-bg-highlight, #eff6ff);
    }
    .notification-item-icon {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      font-size: 14px;
    }
    .ntype-info { background: #dbeafe; }
    .ntype-success { background: #d1fae5; }
    .ntype-warning { background: #fef3c7; }
    .ntype-error { background: #fee2e2; }
    .notification-item-content {
      flex: 1;
      min-width: 0;
    }
    .notification-item-title {
      font-weight: 600;
      font-size: 13px;
      color: var(--color-text, #111827);
      margin-bottom: 2px;
    }
    .notification-item-message {
      font-size: 12px;
      color: var(--color-text-muted, #6b7280);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .notification-item-time {
      font-size: 11px;
      color: var(--color-text-muted, #9ca3af);
      margin-top: 4px;
    }
    .notification-empty {
      padding: 32px 16px;
      text-align: center;
      color: var(--color-text-muted, #9ca3af);
      font-size: 13px;
    }
    .header-avatar-img {
      width: 100%;
      height: 100%;
      border-radius: 50%;
      object-fit: cover;
    }
  `],
})
export class HeaderComponent {
  toggleSidebar = output();
  showNotifications = signal(false);
  showUserMenu = signal(false);

  constructor(
    public auth: AuthService,
    public notifications: NotificationService,
    public profilePhoto: ProfilePhotoService
  ) {}

  get primaryRole(): string {
    const roles = this.auth.currentUser()?.roles ?? [];
    const priority = ['Admin', 'HR', 'Candidate'];
    return priority.find((r) => roles.includes(r)) ?? roles[0] ?? '';
  }

  get isStaffUser(): boolean {
    const roles = this.auth.currentUser()?.roles ?? [];
    return roles.includes('Admin') || roles.includes('HR');
  }

  get initials(): string {
    const name = this.auth.currentUser()?.fullName ?? 'U';
    return name
      .split(' ')
      .map((w) => w[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  toggleNotifications(e: Event): void {
    e.stopPropagation();
    this.showUserMenu.set(false);
    this.showNotifications.update((v) => !v);
    if (!this.showNotifications()) return;
    this.notifications.refresh();
  }

  toggleUserMenu(e: Event): void {
    e.stopPropagation();
    this.showNotifications.set(false);
    this.showUserMenu.update((v) => !v);
  }

  onLogout(): void {
    this.showUserMenu.set(false);
    this.notifications.stopPolling();
    this.auth.logout();
  }

  onMarkAllRead(e: Event): void {
    e.stopPropagation();
    this.notifications.markAllAsRead();
  }

  onClearAll(e: Event): void {
    e.stopPropagation();
    this.notifications.clearAll();
  }

  onNotificationClick(n: any, e: Event): void {
    e.stopPropagation();
    if (!n.isRead) {
      this.notifications.markAsRead(n.userNotificationId);
    }
    if (n.actionUrl) {
      this.showNotifications.set(false);
    }
  }

  getNotificationIcon(type: string): string {
    switch (type?.toLowerCase()) {
      case 'success': return '✓';
      case 'warning': return '⚠';
      case 'error': return '✗';
      default: return 'ℹ';
    }
  }

  timeAgo(dateStr: string): string {
    if (!dateStr) return '';
    const now = Date.now();
    const then = new Date(dateStr).getTime();
    const diff = Math.max(0, now - then);
    const mins = Math.floor(diff / 60000);
    if (mins < 1) return 'Just now';
    if (mins < 60) return `${mins}m ago`;
    const hours = Math.floor(mins / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days < 7) return `${days}d ago`;
    return new Date(dateStr).toLocaleDateString();
  }

  @HostListener('document:click')
  closeDropdowns(): void {
    this.showNotifications.set(false);
    this.showUserMenu.set(false);
  }
}
