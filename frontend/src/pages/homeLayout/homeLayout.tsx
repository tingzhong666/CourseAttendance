import { Card, Layout } from 'antd'
import SideMenu from './SideMenu/SideMenu'
import { Outlet, } from 'react-router'
import { UserProvider } from '../../Contexts/auth'
import Sider from 'antd/es/layout/Sider'
import { MajorProvider } from '../../Contexts/major'

export default () => {
    return (
        <UserProvider>
            <MajorProvider>
                <Layout style={{ minHeight: '100vh' }}>
                    <Sider collapsible >
                        <SideMenu />
                    </Sider>
                    <Layout style={{ padding: 20, minHeight: '100%' }}>

                        <Card
                            style={{
                                width: '100%',
                                boxShadow: '0 1px 2px 0 rgba(0, 0, 0, 0.03), 0 1px 6px -1px rgba(0, 0, 0, 0.32), 0 2px 4px 0 rgba(0, 0, 0, 0.02)',
                                padding: 0,
                                background: "#f5f5f5",
                                height: '100%'
                            }} variant="borderless">
                            <Outlet />
                        </Card>
                    </Layout>
                </Layout>
            </MajorProvider>
        </UserProvider>
    )
}