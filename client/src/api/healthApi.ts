import { apiFetch } from './client'

interface HealthResponse {
  status: string
}

export function checkHealth(): Promise<HealthResponse> {
  return apiFetch<HealthResponse>('/api/health')
}
