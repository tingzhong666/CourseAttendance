import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { Descriptions, DescriptionsProps, Modal, Tag } from 'antd'
import { AttendanceResponseDto, AttendanceStatus, CheckMethod } from '../services/api'
import { CreateUUID } from '../Utils/Utils'
import dayjs from 'dayjs'

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    id: number
}

interface DataRes extends AttendanceResponseDto {
    // 课程名
    courseName: string
    // 学生名
    studentName: string
    // 课程老师
    teacherName: string

}

export default (props: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false)

    // 初始化
    const init = async () => {

        const res = await api.Attendance.apiAttendanceIdGet(props.id)
        const course = await api.Course.apiCourseIdGet(res.data.data?.courseId || -1)
        const student = await api.Account.apiAccountIdGet(res.data.data?.studentId || '')
        const teacher = await api.Account.apiAccountIdGet(course.data.data?.teacherId || '')
        const tmp: DataRes =
            {
                ...res.data.data,
                courseName: course.data.data?.name,
                studentName: student.data.data?.name,
                teacherName: teacher.data.data?.name,
            } as DataRes

        setData(tmp)
    }


    useEffect(() => {
        setIsModalOpen(props.show)
        if (props.show) {
            init()
        }
    }, [props.show])
    useEffect(() => {
        props.showChange(isModalOpen)
    }, [isModalOpen])


    const handleCancel = () => {
        setIsModalOpen(false);
    }

    const handleOk = () => {
        setIsModalOpen(false);
    }

    const [data, setData] = useState<DataRes>({} as DataRes)
    const [checkMethod, setCheckMethod] = useState('')
    useEffect(() => {
        switch (data.checkMethod) {
            case CheckMethod.Normal:
                setCheckMethod('普通')
                break
            case CheckMethod.Password:
                setCheckMethod('密码')
                break
            case CheckMethod.TowCode:
                setCheckMethod('二维码')
                break
        }
    }, [data.checkMethod])
    const [status, setStatus] = useState(<></>)
    useEffect(() => {
        switch (data.status) {
            case AttendanceStatus.None:
                setStatus(<Tag color="red">未处理</Tag>)
                break
            case AttendanceStatus.Ok:
                setStatus(<Tag color="green">已打卡</Tag>)
                break
            case AttendanceStatus.OkTearcher:
                setStatus(<Tag color="green">修正已打卡</Tag>)
                break
            case AttendanceStatus.Leave:
                setStatus(<Tag color="orange">请假</Tag>)
                break
            case AttendanceStatus.Absent:
                setStatus(<Tag color="#f00">缺席</Tag>)
        }
    }, [data.status])

    const items: DescriptionsProps['items'] = [
        {
            key: CreateUUID(),
            label: '课程名',
            children: data.courseName,
        },
        {
            key: CreateUUID(),
            label: '课程老师',
            children: data.teacherName,
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '学生',
            children: data.studentName,
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '开始时间',
            children: dayjs(data.startTime).format('YYYY-MM-DD hh:mm:ss'),
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '结束时间',
            children: dayjs(data.endTime).format('YYYY-MM-DD hh:mm:ss'),
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '类型',
            children: checkMethod,
        },
        {
            key: CreateUUID(),
            label: '状态',
            children: status,
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '备注',
            children: data.remark,
            span: 'filled',
        },
        {
            key: CreateUUID(),
            label: '创建时间',
            children: dayjs(data.createdAt).format('YYYY-MM-DD hh:mm:ss'),
        },
        {
            key: CreateUUID(),
            label: '更新时间',
            children: dayjs(data.updatedAt).format('YYYY-MM-DD hh:mm:ss'),
            span: 'filled',
        },
    ]
    return (
        <Modal
            title='考勤详情'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
        >
            <Descriptions bordered items={items} />
        </Modal>
    )
}