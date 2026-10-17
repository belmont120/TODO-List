import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Login } from './components/login/login';
import { TodoList } from './components/todo-list/todo-list';
import { AuthService } from './services/auth';

@Component({
  imports: [TodoList, Login, MatToolbarModule, MatButtonModule],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly auth = inject(AuthService);
  protected readonly showLogin = signal(false);
}
