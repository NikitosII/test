import { zodResolver } from '@hookform/resolvers/zod'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError, describeError } from '../../api/ApiError'
import { Button } from '../../components/Button'
import { ErrorMessage } from '../../components/ErrorMessage'
import { FormField } from '../../components/FormField'
import { emptyOrderForm, orderFormSchema, today } from './orderSchema'
import type { OrderFormOutput, OrderFormValues } from './orderSchema'
import { createOrder } from './ordersApi'

const FIELD_NAMES = Object.keys(emptyOrderForm) as Array<keyof OrderFormValues>

function isFieldName(key: string): key is keyof OrderFormValues {
  return (FIELD_NAMES as string[]).includes(key)
}

export function CreateOrderPage() {
  const navigate = useNavigate()
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<OrderFormValues, unknown, OrderFormOutput>({
    resolver: zodResolver(orderFormSchema),
    defaultValues: emptyOrderForm,
    mode: 'onBlur',
  })

  function applyServerErrors(error: unknown): void {
    const fieldErrors = error instanceof ApiError ? error.validationErrors : null

    if (fieldErrors === null) {
      setFormError(describeError(error))

      return
    }

    const unmapped: string[] = []

    for (const [key, messages] of Object.entries(fieldErrors)) {
      const message = messages[0]

      if (message === undefined) {
        continue
      }

      if (isFieldName(key)) {
        setError(key, { type: 'server', message })
      } else {
        unmapped.push(message)
      }
    }

    setFormError(unmapped.length > 0 ? unmapped.join(' ') : null)
  }

  async function onSubmit(values: OrderFormOutput): Promise<void> {
    setFormError(null)

    try {
      const created = await createOrder(values)

      void navigate(`/orders/${created.id}`, { replace: true })
    } catch (error) {
      applyServerErrors(error)
    }
  }

  return (
    <section>
      <header className="page__header">
        <div>
          <h1 className="page__title">Новый заказ</h1>
          <p className="page__subtitle">Все поля обязательны. Номер заказа присвоит система.</p>
        </div>

        <Link className="button button--secondary" to="/orders">
          Назад к списку
        </Link>
      </header>

      {formError !== null && <ErrorMessage message={formError} />}

      <form className="form" noValidate onSubmit={handleSubmit(onSubmit)}>
        <fieldset className="form__group" disabled={isSubmitting}>
          <legend className="form__legend">Отправитель</legend>

          <FormField error={errors.senderCity?.message} id="senderCity" label="Город">
            <input
              autoComplete="off"
              className="input"
              id="senderCity"
              placeholder="Москва"
              type="text"
              {...register('senderCity')}
            />
          </FormField>

          <FormField error={errors.senderAddress?.message} id="senderAddress" label="Адрес">
            <input
              autoComplete="off"
              className="input"
              id="senderAddress"
              placeholder="ул. Тверская, д. 1"
              type="text"
              {...register('senderAddress')}
            />
          </FormField>
        </fieldset>

        <fieldset className="form__group" disabled={isSubmitting}>
          <legend className="form__legend">Получатель</legend>

          <FormField error={errors.receiverCity?.message} id="receiverCity" label="Город">
            <input
              autoComplete="off"
              className="input"
              id="receiverCity"
              placeholder="Санкт-Петербург"
              type="text"
              {...register('receiverCity')}
            />
          </FormField>

          <FormField error={errors.receiverAddress?.message} id="receiverAddress" label="Адрес">
            <input
              autoComplete="off"
              className="input"
              id="receiverAddress"
              placeholder="Невский пр-т, д. 28"
              type="text"
              {...register('receiverAddress')}
            />
          </FormField>
        </fieldset>

        <fieldset className="form__group" disabled={isSubmitting}>
          <legend className="form__legend">Груз</legend>

          <FormField
            error={errors.weight?.message}
            hint="До 20 000 кг, не более трёх знаков после запятой. Запятая и точка равнозначны."
            id="weight"
            label="Вес, кг"
          >
            <input
              autoComplete="off"
              className="input"
              id="weight"
              inputMode="decimal"
              placeholder="12,5"
              type="text"
              {...register('weight')}
            />
          </FormField>

          <FormField error={errors.pickupDate?.message} id="pickupDate" label="Дата забора груза">
            <input className="input" id="pickupDate" min={today()} type="date" {...register('pickupDate')} />
          </FormField>
        </fieldset>

        <div className="form__actions">
          <Button disabled={isSubmitting} type="submit">
            {isSubmitting ? 'Создаём…' : 'Создать заказ'}
          </Button>
        </div>
      </form>
    </section>
  )
}
