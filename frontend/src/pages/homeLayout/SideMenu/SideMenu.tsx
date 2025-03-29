import { Button, Menu, MenuProps } from 'antd'
import { SelectInfo } from 'rc-menu/lib/interface'
import React from 'react'
import { useNavigate } from 'react-router'
import { useAuth } from '../../../Contexts/auth'


export default () => {
    const navigate = useNavigate()
    const auth = useAuth()

    type MenuItem = Required<MenuProps>['items'][number];
    const items: MenuItem[] = [
        { key: '/home', label: '首页' },
        {
            key: 'sub1',
            label: '课堂与考勤',
            children: [
                { key: '/courses', label: '课程列表' },
                { key: '/my-courses', label: '我的课程' },
                { key: '/attendance', label: '考勤' },
            ],
        },
        {
            key: 'sub2',
            label: '设置',
            children: [
                { key: '/user-manager', label: '用户管理' },
                { key: '/student-manager', label: '学生管理' },
                { key: '/modify-pw', label: '修改密码' },
                { key: '/modify-userinfo', label: '个人信息修改' },
            ],
        },
        { key: '/grades-manager', label: '班级管理' },
        { key: '/logout', label: '退出登录' },
    ]


    const selecetChange = (x: SelectInfo): void => {
        if (x.key == '/logout') {
            auth.logout()
            return
        }
        navigate(x.key)
    }

    return (
        <div>
            <Menu
                defaultSelectedKeys={['1']}
                defaultOpenKeys={['sub1']}
                mode="inline"
                theme="dark"
                items={items}
                onSelect={x => selecetChange(x)}
            />
        </div>
    )
}