import { Form, Input, Modal } from "antd"
import { useEffect, useState } from "react";
import { MajorsCategoryReqDto, MajorsCategoryResDto } from "../services/api/api";
import * as api from '../services/http/httpInstance'

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
    model: 'put' | 'add' | 'get'
    putId?: number
}

export type MajorsCategoryAddProps = Props;

export default (prop: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [title, setTitle] = useState('');


    // 初始化
    const init = async () => {
        if (prop.model == 'put') {
            var res = await api.MajorsCategory.apiMajorsCategoryIdGet(prop.putId || -1)
            form.setFieldsValue(res.data.data)
            setTitle('修改专业')
        }
        else if (prop.model == 'add') {
            form.resetFields()
            setTitle('新增专业')
        }
        else if (prop.model == 'get') {
            setTitle('专业详情')
        }

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


    // 弹框
    const handleCancel = () => {
        setIsModalOpen(false);
    }

    const handleOk = () => {

        form.submit()
    }

    // 表单
    const [form] = Form.useForm();
    const onFinish = async (values: MajorsCategoryReqDto) => {
        try {
            await form.validateFields()
            setConfirmLoading(true)

            if (prop.model == 'add')
                var res = await api.MajorsCategory.apiMajorsCategoryPost(values)
            else if (prop.model == 'put')
                var res = await api.MajorsCategory.apiMajorsCategoryPut(prop.putId, values)
            prop.onFinish()
            setIsModalOpen(false)
        } catch (error) {

        }
        setConfirmLoading(false)
    }
    return (
        <Modal
            title="新增大专业"
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
            okText='确定'
            cancelText='取消'
        >

            <Form<MajorsCategoryReqDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<MajorsCategoryReqDto> name='name' rules={[{ required: true, message: '不能为空' }]} label='大专业名'>
                    <Input placeholder="大专业名"></Input>
                </Form.Item>
            </Form>
        </Modal>
    )
}