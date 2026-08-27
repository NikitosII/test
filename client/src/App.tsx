import { Navigate, Route, Routes } from 'react-router-dom'
import { CreateOrderPage } from './features/orders/CreateOrderPage'
import { OrderDetailsPage } from './features/orders/OrderDetailsPage'
import { OrdersListPage } from './features/orders/OrdersListPage'

export function App() {
  return (
    <div className="layout">
      <main className="layout__content">
        <Routes>
          <Route element={<Navigate replace to="/orders" />} path="/" />
          <Route element={<OrdersListPage />} path="/orders" />
          <Route element={<CreateOrderPage />} path="/orders/new" />
          <Route element={<OrderDetailsPage />} path="/orders/:id" />
          <Route element={<Navigate replace to="/orders" />} path="*" />
        </Routes>
      </main>
    </div>
  )
}
