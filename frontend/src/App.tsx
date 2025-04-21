import './App.css'
import { ConfigProvider } from 'antd';
import { Outlet } from 'react-router';

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
