import type { ReactNode } from 'react'

interface FormFieldProps {
  id: string
  label: string
  error?: string | undefined
  hint?: string | undefined
  children: ReactNode
}

export function FormField({ id, label, error, hint, children }: FormFieldProps) {
  const hintId = `${id}-hint`
  const errorId = `${id}-error`

  return (
    <div className="field">
      <label className="field__label" htmlFor={id}>
        {label}
        <span aria-hidden="true" className="field__required">
          *
        </span>
      </label>

      {children}

      {hint !== undefined && !error && (
        <p className="field__hint" id={hintId}>
          {hint}
        </p>
      )}

      {error !== undefined && (
        <p className="field__error" id={errorId} role="alert">
          {error}
        </p>
      )}
    </div>
  )
}
