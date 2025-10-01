import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const AdminGuard: CanActivateFn = () => {
  const router = inject(Router);
  const role = localStorage.getItem('role');
  const ok = role === 'Admin';
  if (!ok) router.navigate(['/']);
  return ok;
};
