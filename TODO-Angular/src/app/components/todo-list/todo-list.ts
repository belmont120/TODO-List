import { Component } from '@angular/core';
import { TodoItem } from '../todo-item/todo-item';

@Component({
  imports: [TodoItem],
  selector: 'app-todo-list',
  styleUrl: './todo-list.scss',
  templateUrl: './todo-list.html',
})
export class TodoList {}
