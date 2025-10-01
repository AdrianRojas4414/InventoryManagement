import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const AuthGuard: CanActivateFn = () => {
  const router = inject(Router);
  const logged = !!localStorage.getItem('userId');
  if (!logged) router.navigate(['/login']);
  return logged;
};
