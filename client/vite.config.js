import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'node:path';
export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
    server: {
        port: 5173,
        proxy: {
            '/api': 'http://localhost:5080',
        },
    },
    test: {
        environment: 'happy-dom',
        globals: true,
        include: ['src/**/*.test.{ts,tsx}'],
    },
});
