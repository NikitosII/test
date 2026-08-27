import type { ProblemDetails, ValidationProblemDetails } from './types'

/** Ошибка, о которой сервер сообщил осознанно: есть статус и, возможно, ProblemDetails. */
export class ApiError extends Error {
  readonly status: number
  readonly problem: ProblemDetails | null

  constructor(status: number, problem: ProblemDetails | null, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
  }
  get validationErrors(): Record<string, string[]> | null {
    const problem = this.problem as ValidationProblemDetails | null

    return problem?.errors ?? null
  }
}
export class NetworkError extends Error {
  constructor(cause: unknown) {
    super('Не удалось связаться с сервером. Проверьте подключение.')
    this.name = 'NetworkError'
    this.cause = cause
  }
}

export function describeError(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message
  }

  if (error instanceof NetworkError) {
    return error.message
  }
  return 'Непредвиденная ошибка. Попробуйте ещё раз.'
}
