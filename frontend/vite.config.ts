import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { visualizer } from 'rollup-plugin-visualizer'
import compression from 'vite-plugin-compression'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        visualizer({ // ��ӿ��ӻ��������
            open: true,
            filename: 'dist/stats.html'
        }),
        compression({
            algorithm: 'gzip', // ѹ���㷨/��ʽ
            ext: '.gz',
            threshold: 10240 // ���ڴ˵��ļ���ѹ�� �ֽ�
        })
    ],
    server: {
        port: 7976,
    },
    build: {
        outDir: '../CourseAttendance/wwwroot', // �������Ŀ¼
    },
})
