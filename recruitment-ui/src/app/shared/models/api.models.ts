export interface ApiResponse<T> {
  hasError: boolean;
  decentMessage: string;
  errorDetails: string | null;
  content: T;
}

export interface PagedRequest {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  searchTerm?: string;
  searchProperties?: string[];
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
