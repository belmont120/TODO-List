import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  imports: [
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
  ],
  selector: 'app-login',
  styleUrl: './login.scss',
  templateUrl: './login.html',
})
export class Login {
  private readonly auth = inject(AuthService);

  readonly cancelled = output<void>();

  readonly userName = signal('');
  readonly password = signal('');
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  async submit(): Promise<void> {
    if (!this.userName().trim() || !this.password()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    try {
      await this.auth.login(this.userName().trim(), this.password());
    } catch (error: unknown) {
      this.errorMessage.set(this.getLoginErrorMessage(error));
    } finally {
      this.isSubmitting.set(false);
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }

    private getLoginErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 401) {
        return 'Invalid username or password.';
      }

      if (error.status === 0) {
        return 'Could not reach the server. Please try again.';
      }
    }

    return 'Something went wrong. Please try again.';
  }
}

