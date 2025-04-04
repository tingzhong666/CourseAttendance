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
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);

    useEffect(() => {
        init()

    }, [])


    const init = async () => {
        if (prop.model == 'put') {
            var res = await api.MajorsCategory.apiMajorsCategoryIdGet(prop.putId || -1)
            // console.log(res.data.data)
            // setInitValueForm(res.data.data || {})
            form.setFieldsValue(res.data.data)
        }
        else if (prop.model == 'add') {
            form.resetFields ()
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


    const handleCancel = () => {
        setIsModalOpen(false);
    }

    const handleOk = () => {
        setIsModalOpen(false)

        form.submit()
    }

    const [form] = Form.useForm();
    // const [initValueForm, setInitValueForm] = useState({} as MajorsCategoryResDto)

    const onFinish = async (values: MajorsCategoryReqDto) => {
        setConfirmLoading(true)

        if (prop.model == 'add')
            var res = await api.MajorsCategory.apiMajorsCategoryPost(values)
        else if (prop.model == 'put')
            var res = await api.MajorsCategory.apiMajorsCategoryPut(prop.putId, values)
        setConfirmLoading(false)
        prop.onFinish()
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
            // initialValues={initValueForm}
            >
                <Form.Item<MajorsCategoryReqDto> name='name'>
                    <Input placeholder="大专业名"></Input>
                </Form.Item>
            </Form>
        </Modal>
    )
}