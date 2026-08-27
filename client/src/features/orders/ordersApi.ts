import { request } from '../../api/http'
import type { CreateOrderRequest, OrderListItemResponse, OrderResponse } from '../../api/types'

export function listOrders(signal?: AbortSignal): Promise<OrderListItemResponse[]> {
  return request<OrderListItemResponse[]>('/orders', { signal })
}

export function getOrder(id: string, signal?: AbortSignal): Promise<OrderResponse> {
  return request<OrderResponse>(`/orders/${encodeURIComponent(id)}`, { signal })
}

export function createOrder(order: CreateOrderRequest): Promise<OrderResponse> {
  return request<OrderResponse>('/orders', { method: 'POST', body: order })
}
