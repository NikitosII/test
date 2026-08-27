import { useCallback } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAsyncData } from '../../api/useAsyncData'
import { ErrorMessage } from '../../components/ErrorMessage'
import { Loader } from '../../components/Loader'
import { formatDate, formatWeight } from './format'
import { listOrders } from './ordersApi'

export function OrdersListPage() {
  const navigate = useNavigate()
  const load = useCallback((signal: AbortSignal) => listOrders(signal), [])
  const { state, reload } = useAsyncData(load)

  return (
    <section>
      <header className="page__header">
        <div>
          <h1 className="page__title">Заказы</h1>
        </div>

        <Link className="button button--primary" to="/orders/new">
          Новый заказ
        </Link>
      </header>

      {state.status === 'loading' && <Loader label="Загружаем заказы…" />}

      {state.status === 'error' && <ErrorMessage message={state.message} onRetry={reload} />}

      {state.status === 'success' && state.data.length > 0 && (
        <div className="table-wrapper">
          <table className="table">
            <caption className="visually-hidden">
              Список заказов. Строка открывает карточку заказа.
            </caption>
            <thead>
              <tr>
                <th scope="col">Номер</th>
                <th scope="col">Откуда</th>
                <th scope="col">Куда</th>
                <th scope="col">Вес</th>
                <th scope="col">Дата забора</th>
              </tr>
            </thead>
            <tbody>
              {state.data.map((order) => (
                <tr
                  className="table__row"
                  key={order.id}
                  onClick={() => {
                    void navigate(`/orders/${order.id}`)
                  }}
                  onKeyDown={(event) => {
                    if (event.key === 'Enter') {
                      void navigate(`/orders/${order.id}`)
                    }
                  }}
                  tabIndex={0}
                >
                  <td className="table__cell table__cell--number">{order.number}</td>
                  <td className="table__cell">
                    <span className="table__city">{order.senderCity}</span>
                    <span className="table__address">{order.senderAddress}</span>
                  </td>
                  <td className="table__cell">
                    <span className="table__city">{order.receiverCity}</span>
                    <span className="table__address">{order.receiverAddress}</span>
                  </td>
                  <td className="table__cell table__cell--numeric">{formatWeight(order.weight)}</td>
                  <td className="table__cell">{formatDate(order.pickupDate)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
