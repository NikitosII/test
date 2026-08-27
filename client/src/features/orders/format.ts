const dateFormatter = new Intl.DateTimeFormat('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' })

const weightFormatter = new Intl.NumberFormat('ru-RU', { minimumFractionDigits: 0, maximumFractionDigits: 3 })

const dateTimeFormatter = new Intl.DateTimeFormat('ru-RU', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
})
export function formatDate(isoDate: string): string {
  const [year, month, day] = isoDate.split('-').map(Number)

  if (year === undefined || month === undefined || day === undefined) {
    return isoDate
  }

  return dateFormatter.format(new Date(year, month - 1, day))
}

export function formatDateTime(iso: string): string {
  return dateTimeFormatter.format(new Date(iso))
}

export function formatWeight(weight: number): string {
  return `${weightFormatter.format(weight)} кг`
}
