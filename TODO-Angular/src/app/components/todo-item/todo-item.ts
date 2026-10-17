import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Todo } from '../../models/todo';
import { TodoService } from '../../services/todo';

@Component({
  imports: [FormsModule, MatButtonModule, MatCheckboxModule, MatFormFieldModule, MatIconModule, MatInputModule],
  selector: 'app-todo-item',
  styleUrl: './todo-item.scss',
  templateUrl: './todo-item.html',
})
export class TodoItem {
  private readonly todoService = inject(TodoService);

  readonly todo = input.required<Todo>();
  readonly deleted = output<string>();

  readonly isEditing = signal(false);
  readonly editedTitle = signal('');

  toggle(): void {
    this.todoService.toggle(this.todo().id);
  }

  remove(): void {
    this.deleted.emit(this.todo().id);
  }

  startEdit(): void {
    this.editedTitle.set(this.todo().title);
    this.isEditing.set(true);
  }

  saveEdit(): void {
    if (this.isEditing()) {
      this.todoService.rename(this.todo().id, this.editedTitle());
      this.isEditing.set(false);
    }
  }

  cancelEdit(): void {
    this.isEditing.set(false);
  }
}
