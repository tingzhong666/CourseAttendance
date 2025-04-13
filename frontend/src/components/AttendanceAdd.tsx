import { useEffect, useState } from "react";
import { useAuth } from "../Contexts/auth";
import { Form, FormProps, Input, Modal, Select, SelectProps } from "antd";
import { AttendanceCreateRequestDto, CheckMethod } from "../services/api";
import * as api from '../services/http/httpInstance'
import DateAndTimeForm from "./DateAndTimeForm";
import dayjs, { Dayjs } from "dayjs";

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
}

interface ReqData extends AttendanceCreateRequestDto {
    startTime_: Dayjs
    endTime_: Dayjs
}
export type UserAddProps = Props;

export default (prop: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);


    // 初始化
    const init = async () => {

        onSearchCourse('')
        form.setFieldsValue({ ...formData })

        setOptionsCheckMethod([
            { label: '普通', value: CheckMethod.Normal },
            { label: '密码', value: CheckMethod.Password },
            { label: '二维码', value: CheckMethod.TowCode },
        ])
    }


    useEffect(() => {
        setIsModalOpen(prop.show)
        if (prop.show) {
            init()
        }
    }, [prop.show])
    useEffect(() => {
        prop.showChange(isModalOpen)
    }, [isModalOpen])


    const handleCancel = () => {
        setIsModalOpen(false);
    }

    const handleOk = () => {
        form.submit()
    }

    // 表单
    const [formData, setFormData] = useState<ReqData>({
        startTime_: dayjs(),
        endTime_: dayjs(),
    } as ReqData)
    const [form] = Form.useForm<ReqData>();
    const onFinish = async (values: ReqData) => {
        try {
            await form.validateFields();
            setConfirmLoading(true)

            values.startTime = values.startTime_.toString()
            values.endTime = values.endTime_.toString()
            await api.Attendance.apiAttendancePost(values)

            prop.onFinish()
            setIsModalOpen(false);
        } catch {
        }
        setConfirmLoading(false)
    }

    const onChange: FormProps['onValuesChange'] = (values) => {
        setFormData({ ...form.getFieldsValue() })
    }
    useEffect(() => {
        form.setFieldsValue({ ...formData })
    }, [formData])

    // 课程选择控件
    const [optionsCourse, setOptionsCourse] = useState<SelectProps['options']>()
    const onSearchCourse: (value: string) => void = async (value) => {
        const res = await api.Course.apiCourseGet([], 1, 9999, value)

        const tmp = res.data.data?.dataList?.map(x => {
            return {
                label: x.name,
                value: x.id
            }
        }) || []
        setOptionsCourse(tmp)
    }
    const [optionsCheckMethod, setOptionsCheckMethod] = useState<SelectProps['options']>()

    return (
        <Modal
            title='新增考勤'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
        >
            <Form<ReqData>
                layout="vertical"
                form={form}
                onFinish={onFinish}
                onValuesChange={onChange}
            >
                <Form.Item<ReqData> name="courseId" label="课程"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Select
                        showSearch
                        placeholder="输入搜索课程"
                        options={optionsCourse}
                        // searchValue={courseQ}
                        onSearch={onSearchCourse}
                        optionFilterProp="label"
                    />
                </Form.Item>
                <Form.Item<ReqData> name="checkMethod" label="考勤方式"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Select
                        showSearch
                        placeholder="选择考勤方式"
                        options={optionsCheckMethod}
                    />
                </Form.Item>
                <Form.Item<ReqData> name="startTime_" label="开始时间"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <DateAndTimeForm placeholder="开始" />
                </Form.Item>
                <Form.Item<ReqData> name="endTime_" label="结束时间"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <DateAndTimeForm placeholder="结束" />
                </Form.Item>

                {
                    formData.checkMethod == CheckMethod.Password &&

                    <Form.Item<ReqData> name="passWord" label="密码"
                        rules={[{ required: true, message: '不能为空' }]}
                    >
                        <Input></Input>
                    </Form.Item>
                }
            </Form>
        </Modal>
    )
}