import { Modal, Form, Input, Button } from 'antd'
import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { CourseRequestDto } from '../services/api'

interface Props {
    show: boolean,
    showChange: (show: boolean) => void
}



export default (prop: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [form] = Form.useForm();


    useEffect(() => {
        setIsModalOpen(prop.show)
    }, [prop.show])
    useEffect(() => {
        prop.showChange(isModalOpen)
    }, [isModalOpen])


    const handleCancel = () => {
        setIsModalOpen(false);
    };

    const handleOk = () => {
        setIsModalOpen(false);

        form.submit()
    };

    const onFinish = async (values: CourseRequestDto) => {
        setConfirmLoading(true)

        var res = await api.Course.apiCoursePost(values)

        // if (res.data.code != 1) {
        //     switch (res.data.code) {
        //         case 3:
        //             notification.info({
        //                 message: `请求失败，无效的工号或密码`,
        //                 placement: "topRight",
        //             })
        //             break
        //     }
        //     return;
        // }

        setConfirmLoading(false)
    }
    
    // 名称
    // 老师
    // 地点
    // === 时间
    // 作息表第几节
    // 周几
    // 日期范围
    return (
        <Modal
            title="新增课程"
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
        >
            <Form
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<CourseRequestDto> name="name">
                    <Input placeholder="课程名" />
                </Form.Item>
                <Form.Item<CourseRequestDto> name="teacherId">
                    <Input placeholder="老师" />
                </Form.Item>
            </Form>
        </Modal>
    )
}