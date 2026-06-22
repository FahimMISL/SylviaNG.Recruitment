import { HttpInterceptorFn, HttpContextToken } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { AuthService } from '../services/auth.service';

export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((error) => {
      if (req.context.get(SKIP_ERROR_TOAST)) {
        return throwError(() => error);
      }
      if (error.status === 401) {
        if (auth.isAuthenticated()) {
          auth.logout();
        }
        toast.error('Session expired. Please log in again.');
      } else if (error.status === 403) {
        toast.error('You do not have permission to perform this action.');
      } else if (error.status === 0) {
        toast.error('Unable to connect to the server.');
      } else {
        const message = error.error?.decentMessage || error.error?.errorDetails || 'An unexpected error occurred.';
        toast.error(message);
      }
      return throwError(() => error);
    })
  );
};
