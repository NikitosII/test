/** Ответ, который отдаёт бэкенд на любую предсказуемую ошибку. */
export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  traceId?: string
}

export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>
}

export interface OrderResponse {
  id: string
  number: string
  senderCity: string
  senderAddress: string
  receiverCity: string
  receiverAddress: string
  weight: number
  pickupDate: string
  createdAt: string
}

export type OrderListItemResponse = OrderResponse

export interface CreateOrderRequest {
  senderCity: string
  senderAddress: string
  receiverCity: string
  receiverAddress: string
  weight: number
  pickupDate: string
}
