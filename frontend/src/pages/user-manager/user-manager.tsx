import { Button, message, PaginationProps, Popconfirm, PopconfirmProps, Space, Table } from "antd"
import * as api from '../../services/http/httpInstance'
import { GetUserResDto } from "../../services/api"
import { ColumnsType } from "antd/es/table"
import { useEffect, useState } from "react"
import { useAuth } from "../../Contexts/auth"
import Search, { SearchProps } from "antd/es/input/Search"
import UserAdd from "../../components/userAdd"
export default () => {
    // 查询参数
    const [data, setData] = useState([] as Array<GetUserResDto>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('')

    const auth = useAuth()
    const [addShow, setAddShow] = useState(false)

    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        await getData()
    }
    const getData = async (current_ = current, limit_ = limit, queryStr_ = queryStr) => {

        var res = await api.Account.apiAccountGet(current_, limit_, queryStr_)

        //res.data.data?.dataList
        setData(res.data.data?.dataList || [])
        setTotal(res.data.data?.total || 0)
    }

    // 姓名 工号 身份权限 邮件 手机号
    const columns: ColumnsType<GetUserResDto> = [
        {
            title: '姓名',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: '工号/学号',
            dataIndex: 'userName',
            key: 'userName',
        },
        {
            title: '身份',
            //dataIndex: 'roles',
            key: 'roles',
            render: (_, record) => (record.roles.map(auth.roleMap) + "")
        },
        {
            title: '邮件',
            key: 'email',
            dataIndex: 'email',
        },
        {
            title: '手机号',
            dataIndex: 'phoneNumber',
            key: 'phoneNumber',
        },
        {
            title: '操作',
            key: 'action',
            render: (_, record) => {

                const confirm: PopconfirmProps['onConfirm'] = (e) => {
                    message.success('Click on Yes');
                  };
                return (
                    <Space size="middle">
                        <a>修改</a>
                        <Popconfirm
                            title="提示"
                            description={`确定删除用户 ${record.name} ?`}
                            onConfirm={confirm}
                            okText="确定"
                            cancelText="取消"
                        >
                            <Button danger>删除</Button>
                        </Popconfirm>
                    </Space>
                )
            },
        },
    ]

    // 页码及分页大小变化
    const onPageChange: PaginationProps['onChange'] = async (page, pageSize) => {
        setCurrent(page)
        setLimit(pageSize)


        await getData(page, pageSize);
    }

    const add = (): void => {
        setAddShow(true)
    }
    // 查询
    const onSearch: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setQueryStr(value)
        setCurrent(1)
        await getData(1, undefined, value);
    }
    return (<Space direction='vertical' style={{ width: '100%' }}>
        <Space>
            <Search placeholder="输入查询的姓名/工号/学号" onSearch={onSearch} enterButton />

            {/* 管理员  可以新增 */}
            {auth.user?.roles.includes('Admin') ?
                <Button type='primary' onClick={() => add()}>新增</Button>
                : <></>
            }
        </Space>

        <Table<GetUserResDto>
            columns={columns}
            pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
            dataSource={data}
        />

        <UserAdd show={addShow} showChange={x => setAddShow(x)}/>
    </Space>)
}