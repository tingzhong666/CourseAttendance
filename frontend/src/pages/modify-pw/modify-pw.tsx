import { Button, Form, Input } from "antd"
import { ChangePasswordReqDto, ChangePasswordSelfReqDto } from "../../services/api"
import * as api from '../../services/http/httpInstance'
import { useAuth } from "../../Contexts/auth"

export default () => {
    const auth = useAuth()

    const [form] = Form.useForm<ChangePasswordSelfReqDto>();
    const onFinish = async (values: ChangePasswordSelfReqDto) => {
        try {
            await api.Account.apiAccountChangePasswordSelfPut(values)
        } catch {
        }
    }
    return (<>

        <Form<ChangePasswordSelfReqDto>
            layout="vertical"
            form={form}
            onFinish={onFinish}
        >
            <Form.Item<ChangePasswordSelfReqDto>
                name="currentPassword"
                label="当前密码"
                rules={[{ required: true, message: '不能为空' }]}
            >
                <Input.Password />
            </Form.Item>
            <Form.Item<ChangePasswordSelfReqDto>
                name="newPassword"
                label="新密码"
                rules={[{ required: true, message: '不能为空' }]}
            >
                <Input.Password />
            </Form.Item>
            <Form.Item<ChangePasswordSelfReqDto>
                name="confirmPassword"
                label="再次输入新密码"
                rules={[{ required: true, message: '不能为空' }]}
            >
                <Input.Password />
            </Form.Item>

            <Button htmlType="submit" type="primary">确认</Button>
        </Form>
    </>)
}