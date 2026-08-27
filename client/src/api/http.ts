import { ApiError, NetworkError } from './ApiError'
import type { ProblemDetails } from './types'

const BASE_URL: string = import.meta.env.VITE_API_BASE_URL ?? '/api'

const FALLBACK_MESSAGES: Record<number, string> = {
  400: 'Запрос содержит ошибки.',
  404: 'Запрашиваемые данные не найдены.',
  500: 'Внутренняя ошибка сервера. Попробуйте позже.',
}

interface RequestOptions {
  method?: 'GET' | 'POST'
  body?: unknown
  signal?: AbortSignal
}

export async function request<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
  const { method = 'GET', body, signal } = options

  let response: Response

  try {
    response = await fetch(`${BASE_URL}${path}`, {
      method,
      signal,
      headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') {
      throw error
    }

    throw new NetworkError(error)
  }

  if (!response.ok) {
    throw new ApiError(response.status, await readProblemDetails(response), await buildMessage(response))
  }

  return (await readJson<TResponse>(response)) as TResponse
}

async function readProblemDetails(response: Response): Promise<ProblemDetails | null> {
  const problem = await readJson<ProblemDetails>(response.clone())

  return problem !== null && typeof problem === 'object' ? problem : null
}

async function buildMessage(response: Response): Promise<string> {
  const problem = await readProblemDetails(response)

  return problem?.detail ?? problem?.title ?? FALLBACK_MESSAGES[response.status] ?? `Ошибка ${response.status}.`
}

async function readJson<TValue>(response: Response): Promise<TValue | null> {
  const text = await response.text()

  if (text.length === 0) {
    return null
  }

  try {
    return JSON.parse(text) as TValue
  } catch {
    return null
  }
}
