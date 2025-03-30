import { useEffect, useState } from 'react'
import * as api from '../../services/http/httpInstance'
import { Button, PaginationProps, Space, Table } from 'antd';
import { ColumnsType } from 'antd/es/table';
import { CourseResponseDto, CourseTimeResDto } from '../../services/api';
import Search, { SearchProps } from 'antd/es/input/Search';
import { useAuth } from '../../Contexts/auth';
import { CreateUUID, WeekdayMap } from '../../Utils/Utils';
import CourseAdd from '../../conponents/courseAdd';

interface CourseTimeData {
    // 周几
    weekday: string,
    // 第几节
    section: string,
}

interface CourseData extends CourseResponseDto {
    // 是否选了此课 学生身份用
    isSelected: boolean | null,
    // 是否是我亲自授课 老师身份用
    isTeach: boolean | null,
    // 授课老师的名字
    taecherName: string,
    // 上课时间
    timeDatas: Array<CourseTimeData>,
}

export default () => {
    const auth = useAuth()
    const [data, setData] = useState([] as Array<CourseData>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('')
    const [addShow, setAddShow] = useState(false)

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
            render: (_, record) => {

                var renders = record.timeDatas?.map(x => {

                    return (
                        <div key={CreateUUID()}>
                            {x.weekday}- {x.section}
                        </div>
                    )
                })

                return (<>
                    {renders}
                </>)
            }
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
            // 课程时间
            cd.timeDatas = await timeDataConvert(cd.courseTimes || [])
            return cd
        }) ?? []


        setData(await Promise.all(tmp) || [])
        setTotal(res.data.data?.total || 0)
    }

    // 课程时间转换
    const timeDataConvert = async (dto: CourseTimeResDto[]) => {
        const tmp = dto.map(async v => {
            const res = await api.TimeTable.apiTimeTableIdGet(v.timeTableId || -1)
            return {
                weekday: WeekdayMap(v.weekday),
                section: res.data.data?.name
            } as CourseTimeData
        }) ?? []

        return await Promise.all(tmp)
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
    const add = (): void => {
        setAddShow(true)
    }

    return (<Space direction='vertical' style={{ width: '100%' }}>
        <Space>
            <Search placeholder="输入查询对的课程名" onSearch={onSearch} enterButton />

            {/* 管理员 教务处 老师 可以新增课程 */}
            {auth.user?.roles.includes('Admihn') ||
                auth.user?.roles.includes('Academic') ||
                auth.user?.roles.includes('Teacher') ?
                <Button type='primary' onClick={() => add()}>新增</Button>
                : <></>
            }
        </Space>

        <Table<CourseData>
            columns={columns}
            pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
            dataSource={data}
        />

        <CourseAdd show={addShow} showChange={x => setAddShow(x)} />
    </Space>)
}