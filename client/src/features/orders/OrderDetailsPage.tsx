import { useCallback } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useAsyncData } from '../../api/useAsyncData'
import { ErrorMessage } from '../../components/ErrorMessage'
import { Loader } from '../../components/Loader'
import { formatDate, formatDateTime, formatWeight } from './format'
import { getOrder } from './ordersApi'

export function OrderDetailsPage() {
  const { id = '' } = useParams<{ id: string }>()
  const load = useCallback((signal: AbortSignal) => getOrder(id, signal), [id])
  const { state, reload } = useAsyncData(load)

  return (
    <section>
      <header className="page__header">
       <div>
          <h1 className="page__title">
            {state.status === 'success' ? `Заказ ${state.data.number}` : 'Заказ'}
          </h1>
        </div>

        <Link className="button button--secondary" to="/orders">
          Назад к списку
        </Link>
      </header>

      {state.status === 'loading' && <Loader label="Загружаем заказ…" />}

      {state.status === 'error' && <ErrorMessage message={state.message} onRetry={reload} />}

      {state.status === 'success' && (
        <dl className="details">
          <div className="details__row">
            <dt className="details__term">Номер заказа</dt>
            <dd className="details__value details__value--strong">{state.data.number}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Город отправителя</dt>
            <dd className="details__value">{state.data.senderCity}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Адрес отправителя</dt>
            <dd className="details__value">{state.data.senderAddress}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Город получателя</dt>
            <dd className="details__value">{state.data.receiverCity}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Адрес получателя</dt>
            <dd className="details__value">{state.data.receiverAddress}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Вес груза</dt>
            <dd className="details__value">{formatWeight(state.data.weight)}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Дата забора груза</dt>
            <dd className="details__value">{formatDate(state.data.pickupDate)}</dd>
          </div>
          <div className="details__row">
            <dt className="details__term">Создан</dt>
            <dd className="details__value">{formatDateTime(state.data.createdAt)}</dd>
          </div>
        </dl>
      )}
    </section>
  )
}
