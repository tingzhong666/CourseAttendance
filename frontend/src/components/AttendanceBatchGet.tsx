import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { Descriptions, DescriptionsProps, Modal, QRCode } from 'antd'
import { AttendanceBatchResDto, CheckMethod } from '../services/api'
import { CreateUUID } from '../Utils/Utils'
import dayjs from 'dayjs'

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    id: number
}

interface DataRes extends AttendanceBatchResDto {
    // 课程名
    courseName: string
    // 课程老师
    teacherName: string

}

export default (props: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false)

    // 初始化
    const init = async () => {

        const res = await api.AttendanceBatch.apiAttendanceBatchIdGet(props.id)
        const course = await api.Course.apiCourseIdGet(res.data.data?.courseId || -1)
        const teacher = await api.Account.apiAccountIdGet(course.data.data?.teacherId || '')
        const tmp: DataRes =
            {
                ...res.data.data,
                courseName: course.data.data?.name,
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
        ...(data.checkMethod == CheckMethod.Password ? [{
            key: CreateUUID(),
            label: '密码',
            children: data.passWord,
        }] : []),
        ...(data.checkMethod == CheckMethod.TowCode ? [{
            key: CreateUUID(),
            label: '二维码',
            children: <QRCode value={data.qrCode || '-'} />,
        }] : []),
    ]

    return (
        <Modal
            title='考勤批次详情'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
        >
            <Descriptions bordered items={items} />
        </Modal>
    )
}