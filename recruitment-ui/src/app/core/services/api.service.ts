import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedRequest, PagedResult } from '../../shared/models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, { params })
      .pipe(map((res) => res.content));
  }

  getPaged<T>(endpoint: string, request: PagedRequest): Observable<PagedResult<T>> {
    let params = new HttpParams()
      .set('page', request.page.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.sortBy) params = params.set('sortBy', request.sortBy);
    if (request.sortDirection) params = params.set('sortDirection', request.sortDirection);
    if (request.searchTerm) params = params.set('searchTerm', request.searchTerm);
    if (request.searchProperties?.length) {
      request.searchProperties.forEach((p) => (params = params.append('searchProperties', p)));
    }

    return this.http
      .get<ApiResponse<PagedResult<T>>>(`${this.baseUrl}/${endpoint}/paged`, { params })
      .pipe(map((res) => res.content));
  }

  getById<T>(endpoint: string, id: number): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`)
      .pipe(map((res) => res.content));
  }

  create<T>(endpoint: string, body: unknown): Observable<T> {
    return this.http
      .post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(map((res) => res.content));
  }

  update<T>(endpoint: string, id: number, body: unknown): Observable<T> {
    return this.http
      .put<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`, body)
      .pipe(map((res) => res.content));
  }

  delete(endpoint: string, id: number): Observable<void> {
    return this.http
      .delete<ApiResponse<void>>(`${this.baseUrl}/${endpoint}/${id}`)
      .pipe(map((res) => res.content));
  }
}
