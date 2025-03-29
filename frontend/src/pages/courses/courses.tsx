import { useEffect, useState } from 'react'
import * as api from '../../services/http/httpInstance'
import { Button, PaginationProps, Space, Table } from 'antd';
import { ColumnsType } from 'antd/es/table';
import { data } from 'react-router';
import { CourseResponseDto } from '../../services/api';
import Search, { SearchProps } from 'antd/es/input/Search';
import { useAuth } from '../../Contexts/auth';

interface CourseData extends CourseResponseDto {
    // 是否选了此课 学生身份用
    isSelected: boolean | null,
    // 是否是我亲自授课 老师身份用
    isTeach: boolean | null,
    // 授课老师的名字
    taecherName: string
}

export default () => {
    const auth = useAuth()
    const [data, setData] = useState([] as Array<CourseData>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('')

    useEffect(() => {
        init()
    }, []);

    const init = async () => {
        await getData();
    }

    // 名 老师 地点 时间(每周哪几节节课上) 操作：选课 退课 删除
    const columns: ColumnsType<CourseData> = [
        {
            title: '课程名',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: '老师',
            dataIndex: 'taecherName',
            key: 'teacher',
        },
        {
            title: '地点',
            dataIndex: 'location',
            key: 'location',
        },
        {
            title: '时间',
            key: 'time',
            //dataIndex: 'tags',
            render: () => (<>待定</>)
        },
        {
            title: '操作',
            key: 'action',
            render: (_, record) => {

                // 删除        是授此课的老师 ;教务处 ;管理员     显示
                const isDel = () => {
                    if (auth.user?.roles.includes('Admin'))
                        return true
                    if (auth.user?.roles.includes('Academic'))
                        return true
                    if (auth.user?.roles.includes('Teacher') && record.isTeach)
                        return true
                    return false
                }


                return (
                    <Space size="middle">
                        {auth.user?.roles.includes('Student') && !record.isSelected ? <a onClick={() => selectCourse(record.id)}>选课</a> : <></>}
                        {auth.user?.roles.includes('Student') && record.isSelected ? <a onClick={() => dropCourse(record.id)}>退课</a> : <></>}
                        {isDel() ? <a>删除</a> : <></>}
                    </Space>
                )
            },
        },
    ]

    // 选课
    const selectCourse = async (courseId: number | undefined) => {
        const res = await api.CourseSelection.apiCourseSelectionAddSelfGet(courseId)
        await getData()
    }
    // 退课
    const dropCourse = async (courseId: number | undefined) => {
        const res = await api.CourseSelection.apiCourseSelectionDelSelfGet(courseId)
        await getData()
    }

    // 获取课程列表
    const getData = async () => {
        const res = await api.Course.apiCourseGet(current - 1, limit, queryStr)

        const tmp = res.data.data?.dataList?.map(async x => {
            const cd = x as CourseData
            // 授课老师名字
            const res2 = await api.Account.apiAccountIdGet(cd.teacherId)
            cd.taecherName = res2.data.data?.name || ''
            // 是否选了此课 学生身份用
            cd.isSelected = cd.studentIds.includes(auth.user?.id || '')
            // 是否是我亲自授课 老师身份用
            cd.isTeach = cd.teacherId == auth.user?.id
            return cd
        }) ?? []


        setData(await Promise.all(tmp) || [])
        setTotal(res.data.data?.total || 0)
    }

    // 查询
    const onSearch: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setQueryStr(value)
        setCurrent(1)
        await getData();
    }

    // 页码及分页大小变化
    const onPageChange: PaginationProps['onChange'] = async (page, pageSize) => {
        setCurrent(page)
        setLimit(pageSize)


        await getData();
    }
    return (<Space direction='vertical' style={{ width: '100%' }}>
        <Space>
            <Search placeholder="输入查询对的课程名" onSearch={onSearch} enterButton />
            <Button type='primary'>新增</Button>
        </Space>

        <Table<CourseData>
            columns={columns}
            pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
            dataSource={data}
        />
    </Space>)
}