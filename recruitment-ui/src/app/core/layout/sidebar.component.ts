import { Component, computed, inject, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];
  badge?: number;
}

interface NavSection {
  title: string;
  roles?: string[];
  excludeRoles?: string[];
  items: NavItem[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <aside class="app-sidebar" [class.collapsed]="collapsed()">
      <div class="sidebar-logo">
        <div class="sidebar-logo-icon">R</div>
        <span class="sidebar-logo-text">SylviaNG.Recruitment</span>
      </div>
      <nav class="sidebar-nav">
        @for (section of visibleNavigation(); track section.title) {
          <div class="sidebar-section-label">{{ section.title }}</div>
          @for (item of section.items; track item.route) {
            <a
              class="sidebar-nav-item"
              [routerLink]="item.route"
              routerLinkActive="active"
              [routerLinkActiveOptions]="{ exact: item.route === '/dashboard' }">
              <span class="nav-icon">{{ item.icon }}</span>
              <span class="nav-label">{{ item.label }}</span>
              @if (item.badge) {
                <span class="nav-badge">{{ item.badge }}</span>
              }
            </a>
          }
        }
      </nav>
    </aside>
  `,
})
export class SidebarComponent {
  collapsed = input(false);
  private auth = inject(AuthService);

  private allNavigation: NavSection[] = [
    {
      title: 'Overview',
      items: [
        { label: 'Dashboard', icon: '■', route: '/dashboard' },
      ],
    },
    {
      title: 'Recruitment',
      roles: ['Admin', 'HR'],
      items: [
        { label: 'Job Postings', icon: '▶', route: '/job-postings' },
        { label: 'Applications', icon: '▼', route: '/applications' },
        { label: 'Candidates', icon: '●', route: '/candidates' },
        { label: 'Requisitions', icon: '◆', route: '/requisitions' },
        { label: 'Referrals', icon: '★', route: '/referrals' },
      ],
    },
    {
      title: 'My Applications',
      roles: ['Candidate'],
      excludeRoles: ['Admin', 'HR'],
      items: [
        { label: 'Browse Jobs', icon: '▶', route: '/browse-jobs' },
        { label: 'My Applications', icon: '▼', route: '/my-applications' },
        { label: 'Notifications', icon: '✉', route: '/my-notifications' },
      ],
    },
    {
      title: 'Evaluation',
      roles: ['Admin', 'HR'],
      items: [
        { label: 'Shortlisting', icon: '☑', route: '/shortlisting' },
        { label: 'Assessments', icon: '✎', route: '/assessments' },
        { label: 'Interviews', icon: '☎', route: '/interviews' },
      ],
    },
    {
      title: 'Hiring',
      roles: ['Admin', 'HR'],
      items: [
        { label: 'Verification', icon: '✓', route: '/verification' },
        { label: 'Pre-Boarding', icon: '⚙', route: '/pre-boarding' },
        { label: 'Fitment', icon: '♦', route: '/fitment' },
        { label: 'Onboarding', icon: '★', route: '/onboarding' },
      ],
    },
    {
      title: 'System',
      roles: ['Admin', 'HR'],
      items: [
        { label: 'Career Portal', icon: '☁', route: '/career-portal' },
        { label: 'Notifications', icon: '✉', route: '/notifications' },
        { label: 'Documents', icon: '☷', route: '/documents' },
        { label: 'Analytics', icon: '☷', route: '/analytics' },
        { label: 'Export', icon: '⇩', route: '/export' },
        { label: 'Users & Roles', icon: '⚙', route: '/users', roles: ['Admin'] },
        { label: 'Integrations', icon: '⇄', route: '/integrations', roles: ['Admin'] },
        { label: 'Profile Fields', icon: '⚙', route: '/profile-field-config', roles: ['Admin'] },
        { label: 'Application Fees', icon: '💳', route: '/application-fees', roles: ['Admin', 'HR'] },
      ],
    },
  ];

  visibleNavigation = computed(() => {
    const userRoles = this.auth.currentUser()?.roles ?? [];
    const hasRole = (allowed?: string[]) =>
      !allowed || allowed.some((r) => userRoles.includes(r));
    const excluded = (blocked?: string[]) =>
      !!blocked && blocked.some((r) => userRoles.includes(r));

    return this.allNavigation
      .filter((section) => hasRole(section.roles) && !excluded(section.excludeRoles))
      .map((section) => ({
        ...section,
        items: section.items.filter((item) => hasRole(item.roles)),
      }))
      .filter((section) => section.items.length > 0);
  });
}
