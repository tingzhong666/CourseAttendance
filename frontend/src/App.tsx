import { useEffect, useState } from 'react'
import './App.css'
import { Button, ConfigProvider } from 'antd';
import Login from './pages/login/login';
import HomeLayout from './pages/homeLayout/homeLayout';
import { UserProvider, useAuth } from "./Contexts/auth"
import { Outlet } from 'react-router';
import { useNavigate } from 'react-router';
import { Locale } from 'antd/es/locale';

import zhCN from 'antd/locale/zh_CN';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';

dayjs.locale('zh-cn')


const App = () => {
    return (
        <div className="App">
            <ConfigProvider locale={zhCN}>
                <Outlet />
            </ConfigProvider>
        </div>
    )
}

export default App
