import { useCallback, useEffect, useState } from 'react'
import { describeError } from './ApiError'

export type AsyncState<TData> =
  | { status: 'loading' }
  | { status: 'error'; message: string }
  | { status: 'success'; data: TData }

export function useAsyncData<TData>(load: (signal: AbortSignal) => Promise<TData>) {
  const [attempt, setAttempt] = useState(0)
  const [state, setState] = useState<AsyncState<TData>>({ status: 'loading' })
  const [source, setSource] = useState({ load, attempt })

  if (source.load !== load || source.attempt !== attempt) {
    setSource({ load, attempt })
    setState({ status: 'loading' })
  }

  const reload = useCallback(() => {
    setAttempt((value) => value + 1)
  }, [])

  useEffect(() => {
    const controller = new AbortController()

    load(controller.signal)
      .then((data) => {
        setState({ status: 'success', data })
      })
      .catch((error: unknown) => {
        if (controller.signal.aborted) {
          return
        }

        setState({ status: 'error', message: describeError(error) })
      })

    return () => {
      controller.abort()
    }
  }, [load, attempt])

  return { state, reload }
}
