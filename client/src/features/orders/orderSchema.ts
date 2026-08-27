import { z } from 'zod'

const CITY_MIN_LENGTH = 2
const CITY_MAX_LENGTH = 100
const ADDRESS_MIN_LENGTH = 2
const ADDRESS_MAX_LENGTH = 200
const MAX_WEIGHT = 20_000
const WEIGHT_SCALE = 3

const NUMERIC_PATTERN = /^\d+([.,]\d+)?$/
const ISO_DATE_PATTERN = /^\d{4}-\d{2}-\d{2}$/
export function today(): string {
  const now = new Date()
  const localMidnight = new Date(now.getTime() - now.getTimezoneOffset() * 60_000)

  return localMidnight.toISOString().slice(0, 10)
}

function isNumeric(value: string): boolean {
  return NUMERIC_PATTERN.test(value)
}

function toNumber(value: string): number {
  return Number(value.replace(',', '.'))
}

function decimalPlaces(value: string): number {
  return value.split(/[.,]/)[1]?.length ?? 0
}

function text(label: string, minLength: number, maxLength: number) {
  return z
    .string()
    .trim()
    .min(1, `${label}: поле обязательно для заполнения.`)
    .min(minLength, `${label}: минимум ${minLength} символа.`)
    .max(maxLength, `${label}: максимум ${maxLength} символов.`)
}

const weight = z
  .string()
  .trim()
  .min(1, 'Вес груза обязателен.')
  .refine((value) => isNumeric(value), 'Вес груза должен быть числом.')
  .refine(
    (value) => !isNumeric(value) || decimalPlaces(value) <= WEIGHT_SCALE,
    `Вес груза может содержать не более ${WEIGHT_SCALE} знаков после запятой.`,
  )
  .refine((value) => !isNumeric(value) || toNumber(value) > 0, 'Вес груза должен быть больше 0.')
  .refine(
    (value) => !isNumeric(value) || toNumber(value) <= MAX_WEIGHT,
    `Вес груза не может превышать ${MAX_WEIGHT} кг.`,
  )
  .transform(toNumber)

const pickupDate = z
  .string()
  .min(1, 'Дата забора груза обязательна.')
  .refine((value) => ISO_DATE_PATTERN.test(value), 'Дата забора груза указана в неверном формате.')
  .refine(
    (value) => !ISO_DATE_PATTERN.test(value) || value >= today(),
    'Дата забора груза не может быть раньше сегодняшнего дня.',
  )

export const orderFormSchema = z.object({
  senderCity: text('Город отправителя', CITY_MIN_LENGTH, CITY_MAX_LENGTH),
  senderAddress: text('Адрес отправителя', ADDRESS_MIN_LENGTH, ADDRESS_MAX_LENGTH),
  receiverCity: text('Город получателя', CITY_MIN_LENGTH, CITY_MAX_LENGTH),
  receiverAddress: text('Адрес получателя', ADDRESS_MIN_LENGTH, ADDRESS_MAX_LENGTH),
  weight,
  pickupDate,
})

export type OrderFormValues = z.input<typeof orderFormSchema>
export type OrderFormOutput = z.output<typeof orderFormSchema>

export const emptyOrderForm: OrderFormValues = {
  senderCity: '',
  senderAddress: '',
  receiverCity: '',
  receiverAddress: '',
  weight: '',
  pickupDate: '',
}
