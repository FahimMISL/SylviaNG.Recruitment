import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProfilePhotoService {
  private baseUrl = `${environment.apiUrl}/profile-photo`;

  photoUrl = signal<string | null>(null);
  private currentObjectUrl: string | null = null;

  constructor(private http: HttpClient) {}

  loadMyPhoto(): void {
    this.http.get<any>(`${this.baseUrl}/me`).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        if (data?.keycloakUserId) {
          this.http.get(`${this.baseUrl}/view/${data.keycloakUserId}`, { responseType: 'blob' }).subscribe({
            next: (blob) => {
              if (this.currentObjectUrl) URL.revokeObjectURL(this.currentObjectUrl);
              this.currentObjectUrl = URL.createObjectURL(blob);
              this.photoUrl.set(this.currentObjectUrl);
            },
            error: () => this.photoUrl.set(null),
          });
        } else {
          this.photoUrl.set(null);
        }
      },
      error: () => this.photoUrl.set(null),
    });
  }

  upload(file: File): Promise<boolean> {
    return new Promise((resolve) => {
      const formData = new FormData();
      formData.append('file', file);

      this.http.post<any>(`${this.baseUrl}/upload`, formData).subscribe({
        next: (res) => {
          if (res.hasError) {
            resolve(false);
            return;
          }
          this.loadMyPhoto();
          resolve(true);
        },
        error: () => resolve(false),
      });
    });
  }

  deletePhoto(): Promise<boolean> {
    return new Promise((resolve) => {
      this.http.delete<any>(this.baseUrl).subscribe({
        next: () => {
          this.photoUrl.set(null);
          resolve(true);
        },
        error: () => resolve(false),
      });
    });
  }
}
