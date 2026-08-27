import { Button } from './Button'

interface ErrorMessageProps {
  message: string
  onRetry?: () => void
}

export function ErrorMessage({ message, onRetry }: ErrorMessageProps) {
  return (
    <div className="notice notice--error" role="alert">
      <p className="notice__text">{message}</p>

      {onRetry !== undefined && (
        <Button onClick={onRetry} variant="secondary">
          Повторить
        </Button>
      )}
    </div>
  )
}
