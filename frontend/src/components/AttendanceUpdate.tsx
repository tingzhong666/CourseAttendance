import { Form, FormProps, Input, Modal, Select, SelectProps } from "antd"
import { AttendanceStatus, AttendanceUpdateRequestDto } from "../services/api"
import { useEffect, useState } from "react"
import * as api from '../services/http/httpInstance'

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
    id: number
}
const AttendanceUpdate= (props: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false)
    const [confirmLoading, setConfirmLoading] = useState(false)



    useEffect(() => {
        setIsModalOpen(props.show)
        if (props.show) {
            init()
        }
    }, [props.show])
    useEffect(() => {
        props.showChange(isModalOpen)
    }, [isModalOpen])

    // 初始化
    const init = async () => {

        const res = await api.Attendance.apiAttendanceIdGet(props.id)

        const tmp: AttendanceUpdateRequestDto = {
            status: res.data.data?.status || AttendanceStatus.None,
            remark: res.data.data?.remark || '',
        }

        setFormData(tmp)
        form.setFieldsValue(tmp)

        setOptionsStatus([
            { label: '未处理', value: AttendanceStatus.None },
            { label: '已打卡', value: AttendanceStatus.Ok },
            { label: '修正已打卡', value: AttendanceStatus.OkTearcher },
            { label: '请假', value: AttendanceStatus.Leave },
            { label: '缺席', value: AttendanceStatus.Absent },
        ])
    }



    const handleCancel = () => {
        setIsModalOpen(false);
    }

    const handleOk = () => {
        form.submit()
    }

    const [formData, setFormData] = useState<AttendanceUpdateRequestDto>({} as AttendanceUpdateRequestDto)
    const [form] = Form.useForm<AttendanceUpdateRequestDto>()
    const onFinish = async (values: AttendanceUpdateRequestDto) => {
        try {
            await form.validateFields();
            setConfirmLoading(true)

            await api.Attendance.apiAttendancePut(props.id, values)

            props.onFinish()
            setIsModalOpen(false);
        } catch {
        }
        setConfirmLoading(false)
    }
    const onChange: FormProps['onValuesChange'] = (_values) => {
        setFormData({ ...form.getFieldsValue() })
    }
    useEffect(() => {
        if (!props.show) return

        form.setFieldsValue({ ...formData })
    }, [formData, props.show])


    const [optionsStatus, setOptionsStatus] = useState<SelectProps['options']>()

    return (
        <Modal
            title='修改考勤'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
        >
            <Form<AttendanceUpdateRequestDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
                onValuesChange={onChange}
            >
                <Form.Item<AttendanceUpdateRequestDto> name="status" label="状态"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Select
                        showSearch
                        placeholder="选择状态"
                        options={optionsStatus}
                    />
                </Form.Item>


                <Form.Item<AttendanceUpdateRequestDto> name="remark" label="备注">
                    <Input></Input>
                </Form.Item>
            </Form>
        </Modal>
    )
}

export default AttendanceUpdate