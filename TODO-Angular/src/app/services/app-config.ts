import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from '../models/app-config';

/**
 * Fallback values used when public/config.json hasn't loaded yet (e.g. unit tests)
 * or can't be reached. Production deployments override these by editing config.json
 * in the deployed output — no rebuild required.
 */
const DEFAULT_CONFIG: AppConfig = {
  userServiceUrl: 'http://localhost:5116/api',
  todoServiceUrl: 'http://localhost:5033/api',
};

@Service()
export class AppConfigService {
  private readonly http = inject(HttpClient);

  private config: AppConfig = DEFAULT_CONFIG;

  /** Loaded once at app startup via an app initializer — see app.config.ts. */
  async load(): Promise<void> {
    try {
      const loaded = await firstValueFrom(this.http.get<AppConfig>('config.json'));
      this.config = { ...DEFAULT_CONFIG, ...loaded };
    } catch {
      // config.json missing or unreachable: keep the defaults.
    }
  }

  get userServiceUrl(): string {
    return this.config.userServiceUrl;
  }

  get todoServiceUrl(): string {
    return this.config.todoServiceUrl;
  }
}
