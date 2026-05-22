const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
    this.name = 'ApiError'
  }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
    ...options,
  })

  if (!response.ok) {
    let message = `API error ${response.status}: ${response.statusText}`
    try {
      const body = await response.json()
      if (body?.message) message = body.message
    } catch {
      // body wasn't JSON or was empty — keep the default message
    }
    throw new ApiError(response.status, message)
  }

  // 204 No Content (and any other empty response) — return without parsing.
  if (response.status === 204 || response.headers.get('content-length') === '0') {
    return undefined as T
  }

  return response.json() as Promise<T>
}
