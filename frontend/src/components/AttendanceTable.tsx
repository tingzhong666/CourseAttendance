import { useEffect, useState } from "react"
import * as api from '../services/http/httpInstance'
import { AttendanceResponseDto, AttendanceStatus, CheckMethod, UserRole } from "../services/api"
import Table, { ColumnsType } from "antd/es/table"
import { Button, PaginationProps, Popconfirm, Space, Tag } from "antd"
import { useAuth } from "../Contexts/auth"
import dayjs from "dayjs"
import { useMajor } from "../Contexts/major"
import AttendanceUpdate from "./AttendanceUpdate"
import AttendanceGet from "./AttendanceGet"
import AttendanceSimulation from "./AttendanceSimulation"


interface DataRes extends AttendanceResponseDto {

    // 课程名
    courseName: string
    // 学生名
    studentName: string
    // 考勤类型
    // 状态
}

interface Props {
    // 查询参数
    queryStr?: string // 课程名
    teacherName?: string // 老师名
    studentName?: string // 学生名
    startTime?: dayjs.Dayjs | null  // 时间段
    endTime?: dayjs.Dayjs | null // 时间段
    majorsCategoryId?: number | undefined
    majorsSubcategoriesId?: number | undefined
    batchId?: number | undefined // 批次ID

    // 分页参数
    current?: number

    // 数据重新请求  false变true时
    dataUpdate?: boolean
    onDataUpdated?: () => void
}

export type AttendanceTableProps = Props

export default (props: Props) => {
    const [data, setData] = useState([] as Array<DataRes>)

    // 分页参数
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)

    useEffect(() => {
        if (props.current) setCurrent(props.current)
    }, [props.current])


    // 修改
    const [putShow, setPutShow] = useState(false)
    const [putId, setPutId] = useState<number>(-1)
    // 查看
    const [getShow, setGetShow] = useState(false)
    const [getId, setGetId] = useState<number>(-1)

    const auth = useAuth()


    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        await getData()
    }

    useEffect(() => {
        if (props.dataUpdate)
            getData()
    }, [props.dataUpdate])



    const getData = async (
        current_ = current,
        limit_ = limit,
        queryStr_ = props.queryStr,
        studentName_ = props.studentName,
        teacherName_ = props.teacherName,
        startTime_ = props.startTime?.format() ?? undefined,
        endTime_ = props.endTime?.format() ?? undefined,
        majorsCategoryId_ = props.majorsCategoryId,
        majorsSubcategoriesId_ = props.majorsSubcategoriesId,
        batchId_ = props.batchId
    ) => {

        var res = await api.Attendance.apiAttendanceGet([], studentName_, [], teacherName_, startTime_, endTime_, majorsCategoryId_, majorsSubcategoriesId_, undefined, batchId_, current_, limit_, queryStr_)
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

        if (props.onDataUpdated)
            props.onDataUpdated()
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

                        {/* 考勤模拟 */}
                        {
                            auth.user?.roles.includes(UserRole.Student) &&
                            record.status == AttendanceStatus.None &&
                            < a onClick={() => {
                                setAttendanceSimulationId(record.id)
                                setAttendanceSimulationcheckMethod(record.checkMethod)
                                setAttendanceSimulationShow(true)
                            }}>考勤模拟</a>
                        }
                    </Space >
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



    // 考勤模拟
    const [AttendanceSimulationShow, setAttendanceSimulationShow] = useState(false)
    const [AttendanceSimulationcheckMethod, setAttendanceSimulationcheckMethod] = useState<CheckMethod>(CheckMethod.Normal)
    const [AttendanceSimulationId, setAttendanceSimulationId] = useState<number>(-1)
    return (
        <Space direction='vertical' style={{ width: '100%' }}>
            <Table<DataRes>
                columns={columns}
                pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
                dataSource={data}
                rowKey="id"
            />
            <AttendanceUpdate
                show={putShow}
                showChange={x => setPutShow(x)}
                onFinish={getData}
                id={putId} />


            <AttendanceGet
                show={getShow}
                showChange={x => setGetShow(x)}
                id={getId} />


            <AttendanceSimulation
                checkMethod={AttendanceSimulationcheckMethod}
                id={AttendanceSimulationId}

                show={AttendanceSimulationShow}
                showChange={setAttendanceSimulationShow}
                onFinish={() => getData()} />

        </Space>
    )
}