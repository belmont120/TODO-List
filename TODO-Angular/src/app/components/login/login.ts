import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth';

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
    } catch {
      this.errorMessage.set('Invalid username or password.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
