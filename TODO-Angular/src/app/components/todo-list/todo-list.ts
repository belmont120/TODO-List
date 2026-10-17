import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TodoFilter } from '../../models/todo';
import { TodoService } from '../../services/todo';
import { TodoItem } from '../todo-item/todo-item';
import { Todo } from '../../models/todo';

@Component({
  imports: [
    FormsModule,
    TodoItem,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressBarModule,
  ],
  selector: 'app-todo-list',
  styleUrl: './todo-list.scss',
  templateUrl: './todo-list.html',
})
export class TodoList {
  private readonly todoService = inject(TodoService);

  readonly newTitle = signal('');

  readonly todos = this.todoService.filteredTodos;
  readonly remainingCount = this.todoService.remainingCount;
  readonly filter = this.todoService.filterSignal;
  readonly isLoading = this.todoService.isLoading;
  readonly error = this.todoService.error;

  readonly filters: { value: TodoFilter; label: string }[] = [
    { value: 'all', label: 'All' },
    { value: 'active', label: 'Active' },
    { value: 'completed', label: 'Completed' },
  ];

  addTodo(): void {
    this.todoService.add(this.newTitle());
    this.newTitle.set('');
  }

  removeTodo(id: string): void {
    this.todoService.remove(id);
  }

  setFilter(filter: TodoFilter): void {
    this.todoService.setFilter(filter);
  }

  clearCompleted(): void {
    this.todoService.clearCompleted();
  }
}
