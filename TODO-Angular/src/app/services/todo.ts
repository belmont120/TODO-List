import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Service, computed, effect, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Todo, TodoFilter } from '../models/todo';
import { AppConfigService } from './app-config';
import { AuthService } from './auth';

const STORAGE_KEY = 'todo-list.todos';

interface TodoItemResponse {
  id: string;
  title: string;
  isCompleted: boolean;
  createdAt: string;
}

function fromResponse(item: TodoItemResponse): Todo {
  return {
    id: item.id,
    title: item.title,
    completed: item.isCompleted,
    createdAt: Date.parse(item.createdAt),
  };
}

@Service()
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly appConfig = inject(AppConfigService);

  private readonly todosSignal = signal<Todo[]>(this.loadFromStorage());
  readonly filterSignal = signal<TodoFilter>('all');
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  readonly todos = computed(() => this.todosSignal());
  readonly remainingCount = computed(
    () => this.todosSignal().filter((todo) => !todo.completed).length,
  );
  readonly filteredTodos = computed(() => {
    const filter = this.filterSignal();
    const todos = this.todosSignal();
    if (filter === 'active') {
      return todos.filter((todo) => !todo.completed);
    }
    if (filter === 'completed') {
      return todos.filter((todo) => todo.completed);
    }
    return todos;
  });

  constructor() {
    effect(() => {
      if (this.auth.isAuthenticated()) {
        void this.loadFromServer();
      } else {
        this.todosSignal.set(this.loadFromStorage());
      }
    });
  }

  async add(title: string): Promise<void> {
    const trimmed = title.trim();
    if (!trimmed) {
      return;
    }

    await this.runAction(async () => {
      if (this.auth.isAuthenticated()) {
        const created = await firstValueFrom(
          this.http.post<TodoItemResponse>(
            `${this.appConfig.todoServiceUrl}/todoitems`,
            { title: trimmed },
            { headers: this.authHeaders() },
          ),
        );
        this.todosSignal.set([fromResponse(created), ...this.todosSignal()]);
        return;
      }

      const todo: Todo = {
        id: crypto.randomUUID(),
        title: trimmed,
        completed: false,
        createdAt: Date.now(),
      };
      this.updateLocal([todo, ...this.todosSignal()]);
    });
  }

  async toggle(id: string): Promise<void> {
    const current = this.todosSignal().find((todo) => todo.id === id);
    if (!current) {
      return;
    }

    await this.runAction(async () => {
      if (this.auth.isAuthenticated()) {
        const updated = await firstValueFrom(
          this.http.put<TodoItemResponse>(
            `${this.appConfig.todoServiceUrl}/todoitems/${id}`,
            { title: current.title, isCompleted: !current.completed },
            { headers: this.authHeaders() },
          ),
        );
        this.todosSignal.set(
          this.todosSignal().map((todo) => (todo.id === id ? fromResponse(updated) : todo)),
        );
        return;
      }

      this.updateLocal(
        this.todosSignal().map((todo) => (todo.id === id ? { ...todo, completed: !todo.completed } : todo)),
      );
    });
  }

  async remove(id: string): Promise<void> {
    await this.runAction(async () => {
      if (this.auth.isAuthenticated()) {
        await firstValueFrom(
          this.http.delete<void>(`${this.appConfig.todoServiceUrl}/todoitems/${id}`, { headers: this.authHeaders() }),
        );
        this.todosSignal.set(this.todosSignal().filter((todo) => todo.id !== id));
        return;
      }

      this.updateLocal(this.todosSignal().filter((todo) => todo.id !== id));
    });
  }

  async rename(id: string, title: string): Promise<void> {
    const trimmed = title.trim();
    if (!trimmed) {
      return;
    }
    const current = this.todosSignal().find((todo) => todo.id === id);
    if (!current) {
      return;
    }

    await this.runAction(async () => {
      if (this.auth.isAuthenticated()) {
        const updated = await firstValueFrom(
          this.http.put<TodoItemResponse>(
            `${this.appConfig.todoServiceUrl}/todoitems/${id}`,
            { title: trimmed, isCompleted: current.completed },
            { headers: this.authHeaders() },
          ),
        );
        this.todosSignal.set(
          this.todosSignal().map((todo) => (todo.id === id ? fromResponse(updated) : todo)),
        );
        return;
      }

      this.updateLocal(
        this.todosSignal().map((todo) => (todo.id === id ? { ...todo, title: trimmed } : todo)),
      );
    });
  }

  async clearCompleted(): Promise<void> {
    await this.runAction(async () => {
      if (this.auth.isAuthenticated()) {
        await firstValueFrom(
          this.http.delete<void>(`${this.appConfig.todoServiceUrl}/todoitems/completed`, { headers: this.authHeaders() }),
        );
        this.todosSignal.set(this.todosSignal().filter((todo) => !todo.completed));
        return;
      }

      this.updateLocal(this.todosSignal().filter((todo) => !todo.completed));
    });
  }

  setFilter(filter: TodoFilter): void {
    this.filterSignal.set(filter);
  }

  private async loadFromServer(): Promise<void> {
    this.isLoading.set(true);
    await this.runAction(async () => {
      const items = await firstValueFrom(
        this.http.get<TodoItemResponse[]>(`${this.appConfig.todoServiceUrl}/todoitems`, { headers: this.authHeaders() }),
      );
      this.todosSignal.set(items.map(fromResponse));
    });
    this.isLoading.set(false);
  }

  private async runAction(action: () => Promise<void>): Promise<void> {
    try {
      await action();
      this.error.set(null);
    } catch {
      this.error.set('Could not reach the server. Please try again.');
    }
  }

  private authHeaders(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  private updateLocal(todos: Todo[]): void {
    this.todosSignal.set(todos);
    this.saveToStorage(todos);
  }

  private loadFromStorage(): Todo[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as Todo[]) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(todos: Todo[]): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(todos));
  }
}
