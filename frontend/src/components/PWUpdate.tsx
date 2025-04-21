import { Form, Input, Modal } from "antd"
import { useEffect, useState } from "react"
import * as api from '../services/http/httpInstance'
import { ResetPasswordReqDto } from "../services/api"

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    putId?: string
}
export type UserAddProps = Props;

const PWUpdate = (prop: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);


    useEffect(() => {
        setIsModalOpen(prop.show)

        if (prop.show) {
            form.resetFields()
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
    const [form] = Form.useForm<ResetPasswordReqDto>();
    const onFinish = async (values: ResetPasswordReqDto) => {
        try {
            values.userId = prop.putId + ''
            await api.Account.apiAccountChangePasswordPut(values)
            setIsModalOpen(false);
        } catch {
        }
        setConfirmLoading(false)
    }

    return (
        <Modal
            title='更新密码'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
        >
            <Form<ResetPasswordReqDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<ResetPasswordReqDto>
                    name="newPassword"
                    label="新密码"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Input.Password />
                </Form.Item>
                <Form.Item<ResetPasswordReqDto>
                    name="confirmPassword"
                    label="再次输入新密码"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Input.Password />
                </Form.Item>
            </Form>
        </Modal>
    )
}

export default PWUpdate