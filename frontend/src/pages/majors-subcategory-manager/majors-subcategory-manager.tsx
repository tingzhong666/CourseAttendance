import { useEffect, useRef, useState } from "react"
import { useAuth } from "../../Contexts/auth"
import { MajorsSubcategoryResDto } from "../../services/api"
import * as api from '../../services/http/httpInstance'
import Table, { ColumnsType } from "antd/es/table"
import { Button, PaginationProps, Popconfirm, Space } from "antd"
import { SearchProps } from "antd/es/input"
import MajorsSubcategoryAdd, { MajorsSubcategoryAddProps } from "../../components/MajorsSubcategoryAdd"
import Search from "antd/es/input/Search"
import { MajorsSubcategoryData, useMajor } from "../../Contexts/major"

export default () => {
    const auth = useAuth()
    // 查询参数
    const [data, setData] = useState([] as Array<MajorsSubcategoryData>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('')

    const major = useMajor()
    // 是否有增改删操作
    const [isChange, setIsChange] = useState(false)
        const isChangeRef = useRef(isChange);
    
        useEffect(() => {
            isChangeRef.current = isChange;
        }, [isChange]);

    useEffect(() => {
        init()
        return () => {
            if (isChangeRef.current)
                major.update()
        }
    }, [])


    const init = async () => {
        await getData()
    }
    const getData = async (current_ = current, limit_ = limit, queryStr_ = queryStr) => {
        var res = await api.MajorsSubcategory.apiMajorsSubcategoryGet(undefined,current_, limit_, queryStr_)
        const tmp2 = res.data.data?.dataList?.map(x => {
            const tmp = x as MajorsSubcategoryData
            return tmp
        })
        setData(tmp2 || [])
        setTotal(res.data.data?.total || 0)
    }


    const del = async (v: MajorsSubcategoryResDto) => {
        await api.MajorsSubcategory.apiMajorsSubcategoryDelete(v.id)

        await getData()
        setIsChange(true)
    }
    const columns: ColumnsType<MajorsSubcategoryResDto> = [
        {
            title: '名称',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: '父专业',
            dataIndex: 'parentName',
            key: 'parentName',
            render: (_, record) => {
                const majorsCategory = major.majorsCategories.find(v => v.id == record.majorsCategoriesId)
                return  majorsCategory?.name || ''
            }
        },
        {
            title: '操作',
            key: 'action',
            render: (_, record) => {

                return (
                    <Space size="middle">
                        <a onClick={() => put(record.id || -1)}>修改</a>
                        <Popconfirm
                            title="提示"
                            description={`确定删除专业 ${record.name} ?`}
                            onConfirm={() => del(record)}
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
    // 查询
    const onSearch: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        await getData(1, undefined, value);
    }
    // 页码及分页大小变化
    const onPageChange: PaginationProps['onChange'] = async (page, pageSize) => {
        setCurrent(page)
        setLimit(pageSize)

        await getData(page, pageSize);
    }

    const [addShow, setAddShow] = useState(false)
    const [putId, setPutId] = useState(-1)
    const [addModel, setAddModel] = useState<MajorsSubcategoryAddProps['model']>('add')

    const add = () => {
        setAddModel('add')
        setAddShow(true)
        setIsChange(true)
    }
    const put = (id: number) => {
        setAddModel('put')
        setAddShow(true)
        setPutId(id)
        setIsChange(true)
    }
    return (<Space direction='vertical' style={{ width: '100%' }}>
        <Space>
            <Search placeholder="输入查询的专业名" onSearch={onSearch} enterButton />

            {/* 管理员  可以新增 */}
            {auth.user?.roles.includes('Admin') ?
                <Button type='primary' onClick={add}>新增</Button>
                : <></>
            }
        </Space>

        <Table<MajorsSubcategoryResDto>
            columns={columns}
            pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
            dataSource={data}
            rowKey="id"
        />

        <MajorsSubcategoryAdd show={addShow} showChange={x => setAddShow(x)} onFinish={getData} model={addModel} putId={putId} />
    </Space>)
}