interface LoaderProps {
  label?: string
}

export function Loader({ label = 'Загрузка…' }: LoaderProps) {
  return (
    <div aria-live="polite" className="loader" role="status">
      <span className="loader__spinner" />
      <span>{label}</span>
    </div>
  )
}
