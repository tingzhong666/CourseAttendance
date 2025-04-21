import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { visualizer } from 'rollup-plugin-visualizer'
import compression from 'vite-plugin-compression'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        visualizer({ // 添加可视化分析插件
            open: true,
            filename: 'dist/stats.html'
        }),
        compression({
            algorithm: 'gzip', // 压缩算法/格式
            ext: '.gz',
            threshold: 10240 // 大于此的文件被压缩 字节
        })
    ],
    server: {
        port: 7976,
    },
    build: {
        outDir: '../CourseAttendance/wwwroot', // 设置输出目录
    },
})
