import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { resolve } from "path"

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 7976,
    }
})
