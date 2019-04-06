import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  secretActivated = false;

  redirectUrl: string;

  activateSecret(): Observable<boolean> {
    return of(true).pipe(
      delay(1000),
      tap(val => this.secretActivated = true)
    );
  }

  deactivateSecret(): void {
    this.secretActivated = false;
  }
}
