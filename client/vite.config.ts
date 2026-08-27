import react from '@vitejs/plugin-react'
import { defineConfig, loadEnv } from 'vite'

const DEV_API_TARGET = 'http://localhost:5134'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), 'VITE_')

  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: env.VITE_DEV_API_TARGET ?? DEV_API_TARGET,
          changeOrigin: true,
        },
      },
    },
  }
})
