import { ChangeEvent, useEffect, useState } from "react"
import * as api from '../../services/http/httpInstance'
import { AttendanceResponseDto, AttendanceStatus, CheckMethod, UserRole } from "../../services/api"
import Table, { ColumnsType } from "antd/es/table"
import { Button, DatePicker, Form, PaginationProps, Popconfirm, Space, Tag, TimePicker } from "antd"
import { SearchProps } from "antd/es/input"
import Search from "antd/es/input/Search"
import { useAuth } from "../../Contexts/auth"
import dayjs, { Dayjs } from "dayjs"
import AttendanceAdd from "../../components/AttendanceAdd"
import AttendanceUpdate from "../../components/AttendanceUpdate"
import AttendanceGet from "../../components/AttendanceGet"


interface DataRes extends AttendanceResponseDto {

    // 课程名
    courseName: string
    // 学生名
    studentName: string
    // 考勤类型
    // 状态
}

export default () => {
    // 查询参数
    const [data, setData] = useState([] as Array<DataRes>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('') // 课程名
    const [teacherName, setTeacherName] = useState('') // 老师名
    const [studentName, setStudentName] = useState('') // 学生名
    const [startTime, setStartTime] = useState<dayjs.Dayjs | null>(null) // 时间段
    const [endTime, setEndTime] = useState<dayjs.Dayjs | null>(null) // 时间段


    const auth = useAuth()


    // 新增
    const [addShow, setAddShow] = useState(false)
    // 修改
    const [putShow, setPutShow] = useState(false)
    const [putId, setPutId] = useState<number>(-1)
    // 查看
    const [getShow, setGetShow] = useState(false)
    const [getId, setGetId] = useState<number>(-1)



    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        await getData()
    }
    const getData = async (current_ = current, limit_ = limit, queryStr_ = queryStr, studentName_ = studentName, teacherName_ = teacherName, startTime_ = startTime, endTime_ = endTime) => {
        var res = await api.Attendance.apiAttendanceGet([], studentName_, [], teacherName_, startTime_?.format() ?? undefined, endTime_?.format() ?? undefined, current_, limit_, queryStr_)
        const tmp = res.data.data?.dataList?.map(async x => {
            const course = await api.Course.apiCourseIdGet(x.courseId)
            const student = await api.Account.apiAccountIdGet(x.studentId)
            return {
                ...x,
                courseName: course.data.data?.name,
                studentName: student.data.data?.name
            } as DataRes
        }) || []
        const tmp2 = await Promise.all(tmp)
        setData(tmp2 || [])
        setTotal(res.data.data?.total || 0)
    }

    // 姓名 工号 身份权限 邮件 手机号
    const columns: ColumnsType<DataRes> = [
        {
            title: '课程名',
            dataIndex: 'courseName',
            key: 'courseName',
        },
        {
            title: '开始时间',
            dataIndex: 'startTime',
            key: 'startTime',
            render: (_, data) => dayjs(data.startTime).format('YYYY-MM-DD HH:mm:ss')
        },
        {
            title: '结束时间',
            dataIndex: 'endTime',
            key: 'endTime',
            render: (_, data) => dayjs(data.endTime).format('YYYY-MM-DD HH:mm:ss')
        },
        {
            title: '类型',
            key: 'checkMethod',
            dataIndex: 'checkMethod',
            render: (_, data) => {
                switch (data.checkMethod) {
                    case CheckMethod.Normal:
                        return '普通'
                    case CheckMethod.Password:
                        return '密码'
                    case CheckMethod.TowCode:
                        return '二维码'
                }
            }
        },
        {
            title: '状态',
            dataIndex: 'status',
            key: 'status',
            render: (_, data) => {
                switch (data.status) {
                    case AttendanceStatus.None:
                        return <Tag color="red">未处理</Tag>
                    case AttendanceStatus.Ok:
                        return <Tag color="green">已打卡</Tag>
                    case AttendanceStatus.OkTearcher:
                        return <Tag color="green">修正已打卡</Tag>
                    case AttendanceStatus.Leave:
                        return <Tag color="orange">请假</Tag>
                    case AttendanceStatus.Absent:
                        return <Tag color="#f00">缺席</Tag>
                }
            }
        },
        {
            title: '操作',
            key: 'action',
            render: (_, record) => {

                return (
                    <Space size="middle">
                        <a onClick={() => get(record.id)}>查看</a>



                        <a onClick={() => put(record.id)}>修改</a>
                        <Popconfirm
                            title="提示"
                            description={`确定删除此考勤?`}
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

    // 页码及分页大小变化
    const onPageChange: PaginationProps['onChange'] = async (page, pageSize) => {
        setCurrent(page)
        setLimit(pageSize)


        await getData(page, pageSize);
    }

    // 条目操作
    const add = async () => {
        setAddShow(true)
    }
    const del = async (v: AttendanceResponseDto) => {
        await api.Attendance.apiAttendanceIdDelete(v.id)
        await getData()
    }
    const put = async (id: number) => {
        setPutShow(true)
        setPutId(id)
    }
    const get = async (id: number) => {
        setGetShow(true)
        setGetId(id)
    }
    // 查询
    const onChangeCourse: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setQueryStr(e.target.value)
    }
    const onSearchCourse: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setQueryStr(value)
        setCurrent(1)
        await getData(1, undefined, value);
    }
    const onChangeTeacher: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setTeacherName(e.target.value)
    }
    const onSearchTeacher: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setTeacherName(value)
        setCurrent(1)
        await getData(1, undefined, undefined, undefined, value);
    }
    const onChangeStudent: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setStudentName(e.target.value)
    }
    const onSearchStudent: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setStudentName(value)
        setCurrent(1)
        await getData(1, undefined, undefined, value);
    }
    const onChangeStartDate: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setStartTime(null)
            return
        }
        let tmp = startTime?.clone() ?? dayjs()
        tmp = tmp?.year(date.year())
        tmp = tmp?.month(date.month())
        tmp = tmp?.date(date.date())
        setStartTime(tmp || null)
    }
    const onChangeStartTime: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setStartTime(null)
            return
        }
        let tmp = startTime?.clone() ?? dayjs()
        tmp = tmp?.hour(date.hour())
        tmp = tmp?.minute(date.minute())
        tmp = tmp?.second(date.second())
        setStartTime(tmp || null)
    }
    const onChangeEndDate: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setEndTime(null)
            return
        }
        let tmp = endTime?.clone() ?? dayjs()
        tmp = tmp?.year(date.year())
        tmp = tmp?.month(date.month())
        tmp = tmp?.date(date.date())
        setEndTime(tmp || null)
    }
    const onChangeEndTime: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setEndTime(null)
            return
        }
        let tmp = endTime?.clone() ?? dayjs()
        tmp = tmp?.hour(date.hour())
        tmp = tmp?.minute(date.minute())
        tmp = tmp?.second(date.second())
        setEndTime(tmp || null)
    }
    const onSearch = async () => {
        setCurrent(1)
        await getData(1);
    }

    return (
        <Space direction='vertical' style={{ width: '100%' }}>

            <Space>
                <Search placeholder="输入查询的课程名" onSearch={onSearchCourse} enterButton  onChange={onChangeCourse}/>

                {(auth.user?.roles.includes(UserRole.Admin) ||
                    auth.user?.roles.includes(UserRole.Academic) ||
                    auth.user?.roles.includes(UserRole.Student)) &&
                    <Search placeholder="输入查询的老师名" onSearch={onSearchTeacher} enterButton onChange={onChangeTeacher} />
                }

                {(auth.user?.roles.includes(UserRole.Admin) ||
                    auth.user?.roles.includes(UserRole.Academic) ||
                    auth.user?.roles.includes(UserRole.Teacher)) &&
                    <Search placeholder="输入查询的学生名" onSearch={onSearchStudent} enterButton onChange={onChangeStudent}/>
                }


                <DatePicker onChange={onChangeStartDate} placeholder='开始日期' />
                <TimePicker onChange={onChangeStartTime} defaultOpenValue={dayjs('00:00:00', 'HH:mm:ss')} />
                -
                <DatePicker onChange={onChangeEndDate} placeholder='结束日期' />
                <TimePicker onChange={onChangeEndTime} defaultOpenValue={dayjs('00:00:00', 'HH:mm:ss')} />

                <Button type='primary' onClick={() => onSearch()}>查询</Button>

                {/* 管理员 教务处 老师  可以新增 */}
                {auth.user?.roles.includes(UserRole.Admin) ||
                    auth.user?.roles.includes(UserRole.Teacher) ||
                    auth.user?.roles.includes(UserRole.Academic)
                    ?
                    <Button type='primary' onClick={() => add()}>新增</Button>
                    : <></>
                }
            </Space>

            <Table<DataRes>
                columns={columns}
                pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
                dataSource={data}
                rowKey="id"
            />

            <AttendanceAdd
                show={addShow}
                showChange={x => setAddShow(x)}
                onFinish={getData} />

            <AttendanceUpdate
                show={putShow}
                showChange={x => setPutShow(x)}
                onFinish={getData}
                id={putId} />


            <AttendanceGet
                show={getShow}
                showChange={x => setGetShow(x)}
                id={getId} />

        </Space>
    )
}