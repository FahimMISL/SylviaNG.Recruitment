import { Component, input, output, signal, computed, OnInit, OnChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PagedResult } from '../models/api.models';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  type?: 'text' | 'badge' | 'date' | 'status' | 'number';
  badgeMap?: Record<string, string>;
}

export interface TableAction {
  label: string;
  icon?: string;
  class?: string;
  handler: (row: any) => void;
}

export interface PageChangeEvent {
  page: number;
  pageSize: number;
  searchTerm: string;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="table-container">
      <div class="table-toolbar">
        <div class="table-toolbar-left">
          <div class="table-search">
            <span>&#128269;</span>
            <input
              type="text"
              [placeholder]="searchPlaceholder()"
              [(ngModel)]="searchTerm"
              (keyup.enter)="onSearch()" />
          </div>
        </div>
        <div class="table-toolbar-right">
          <select class="form-control" style="width:auto" [(ngModel)]="pageSize" (change)="onPageSizeChange()">
            <option [value]="10">10 per page</option>
            <option [value]="25">25 per page</option>
            <option [value]="50">50 per page</option>
          </select>
        </div>
      </div>

      @if (loading()) {
        <div class="card-body">
          @for (i of [1,2,3,4]; track i) {
            <div class="skeleton skeleton-text" [style.width]="(100 - i * 10) + '%'"></div>
          }
        </div>
      } @else if (data() && data()!.data.length > 0) {
        <table class="data-table">
          <thead>
            <tr>
              @for (col of columns(); track col.key) {
                <th
                  [class.sorted]="sortBy === col.key"
                  (click)="col.sortable !== false ? onSort(col.key) : null"
                  [style.cursor]="col.sortable !== false ? 'pointer' : 'default'">
                  {{ col.label }}
                  @if (sortBy === col.key) {
                    <span>{{ sortDirection === 'asc' ? ' ▲' : ' ▼' }}</span>
                  }
                </th>
              }
              @if (actions().length) {
                <th>Actions</th>
              }
            </tr>
          </thead>
          <tbody>
            @for (row of data()!.data; track $index) {
              <tr>
                @for (col of columns(); track col.key) {
                  <td>
                    @switch (col.type) {
                      @case ('badge') {
                        <span class="badge" [class]="'badge-' + getBadgeClass(row[col.key], col.badgeMap)">
                          {{ row[col.key] }}
                        </span>
                      }
                      @case ('status') {
                        @if (row[col.key]) {
                          <span class="badge badge-success">Active</span>
                        } @else {
                          <span class="badge badge-neutral">Inactive</span>
                        }
                      }
                      @case ('date') {
                        {{ formatDate(row[col.key]) }}
                      }
                      @case ('number') {
                        {{ row[col.key]?.toLocaleString() ?? '-' }}
                      }
                      @default {
                        {{ row[col.key] ?? '-' }}
                      }
                    }
                  </td>
                }
                @if (actions().length) {
                  <td>
                    <div class="action-buttons">
                      @for (action of actions(); track action.label) {
                        <button
                          class="action-btn"
                          [class.action-edit]="action.label === 'Edit'"
                          [class.action-delete]="action.label === 'Delete'"
                          [title]="action.label"
                          (click)="action.handler(row)">
                          @if (action.icon) {
                            <span [innerHTML]="action.icon"></span>
                          } @else if (action.label === 'Edit') {
                            <span>&#9998;</span>
                          } @else if (action.label === 'Delete') {
                            <span>&#128465;</span>
                          } @else {
                            {{ action.label }}
                          }
                        </button>
                      }
                    </div>
                  </td>
                }
              </tr>
            }
          </tbody>
        </table>

        <div class="table-pagination">
          <div class="pagination-info">
            Showing {{ (data()!.pageNumber - 1) * data()!.pageSize + 1 }}
            to {{ min(data()!.pageNumber * data()!.pageSize, data()!.totalCount) }}
            of {{ data()!.totalCount }} entries
          </div>
          <div class="pagination-controls">
            <button class="pagination-btn" [disabled]="currentPage <= 1" (click)="goToPage(currentPage - 1)">
              &lt;
            </button>
            @for (p of pageNumbers(); track p) {
              <button class="pagination-btn" [class.active]="p === currentPage" (click)="goToPage(p)">
                {{ p }}
              </button>
            }
            <button class="pagination-btn" [disabled]="currentPage >= totalPages()" (click)="goToPage(currentPage + 1)">
              &gt;
            </button>
          </div>
        </div>
      } @else {
        <div class="empty-state">
          <div class="empty-state-icon">&#9744;</div>
          <div class="empty-state-title">{{ emptyTitle() }}</div>
          <div class="empty-state-text">{{ emptyText() }}</div>
        </div>
      }
    </div>
  `,
  styles: [`
    .action-buttons {
      display: flex;
      align-items: center;
      gap: 6px;
    }
    .action-btn {
      width: 32px;
      height: 32px;
      border-radius: 6px;
      border: 1px solid var(--color-border, #374151);
      background: transparent;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      font-size: 14px;
      transition: all 0.15s;
      color: var(--color-text-muted, #9ca3af);
    }
    .action-btn:hover {
      background: var(--color-bg-secondary, #1f2937);
    }
    .action-btn.action-edit {
      color: #3b82f6;
      border-color: #3b82f640;
    }
    .action-btn.action-edit:hover {
      background: #3b82f618;
    }
    .action-btn.action-delete {
      color: #ef4444;
      border-color: #ef444440;
    }
    .action-btn.action-delete:hover {
      background: #ef444418;
    }
  `],
})
export class DataTableComponent {
  columns = input.required<TableColumn[]>();
  data = input<PagedResult<any> | null>(null);
  loading = input(false);
  actions = input<TableAction[]>([]);
  searchPlaceholder = input('Search...');
  emptyTitle = input('No records found');
  emptyText = input('Data will appear here once records are created.');

  pageChange = output<PageChangeEvent>();

  searchTerm = '';
  currentPage = 1;
  pageSize = 10;
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  min = Math.min;

  totalPages = computed(() => this.data()?.totalPages ?? 0);

  pageNumbers(): number[] {
    const total = this.totalPages();
    const pages: number[] = [];
    const start = Math.max(1, this.currentPage - 2);
    const end = Math.min(total, this.currentPage + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }

  onSearch(): void {
    this.currentPage = 1;
    this.emitChange();
  }

  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.emitChange();
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.emitChange();
  }

  goToPage(page: number): void {
    this.currentPage = page;
    this.emitChange();
  }

  getBadgeClass(value: string, badgeMap?: Record<string, string>): string {
    if (badgeMap && badgeMap[value]) return badgeMap[value];
    const lower = (value ?? '').toString().toLowerCase();
    if (['active', 'approved', 'completed', 'passed', 'open', 'accepted'].includes(lower)) return 'success';
    if (['pending', 'draft', 'shortlisted', 'in progress', 'scheduled'].includes(lower)) return 'warning';
    if (['rejected', 'closed', 'failed', 'cancelled', 'expired'].includes(lower)) return 'danger';
    if (['under review', 'new', 'submitted'].includes(lower)) return 'info';
    return 'neutral';
  }

  formatDate(value: string): string {
    if (!value) return '-';
    try {
      return new Date(value).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    } catch {
      return value;
    }
  }

  private emitChange(): void {
    this.pageChange.emit({
      page: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
    });
  }
}
