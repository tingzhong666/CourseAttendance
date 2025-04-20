import { useEffect, useState } from "react"
import * as api from '../services/http/httpInstance'
import { Form, Input, Modal, Select, SelectProps } from "antd"
import { MajorsCategoryResDto, MajorsSubcategoryReqDto } from "../services/api"
import { useMajor } from "../Contexts/major"

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
    model: 'put' | 'add' | 'get'
    putId?: number
}
export type MajorsSubcategoryAddProps = Props;


export default (prop: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [title, setTitle] = useState('');

    // 专业数据
    const major = useMajor()

    // 初始化
    const init = async () => {
        if (prop.model == 'put') {
            var res = await api.MajorsSubcategory.apiMajorsSubcategoryIdGet(prop.putId || -1)


            const label = major.majorsCategories.find(x => x.id == res.data.data?.majorsCategoriesId)?.name
            setMajorsCategories([{
                label,
                value: res.data.data?.id
            }])
            setMajorsCategoriesValue(res.data.data?.id + '')
            setMajorsCategoriesSearchValue(label || '')

            form.setFieldsValue(res.data.data)
            setTitle('修改专业')
        }
        else if (prop.model == 'add') {
            form.resetFields()
            setTitle('新增专业')
            onSearchMajorsCategory('')
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
    const [form] = Form.useForm()
    const onFinish = async (values: MajorsSubcategoryReqDto) => {
        try {
            await form.validateFields()
            setConfirmLoading(true)

            if (prop.model == 'add')
                var res = await api.MajorsSubcategory.apiMajorsSubcategoryPost(values)
            else if (prop.model == 'put')
                var res = await api.MajorsSubcategory.apiMajorsSubcategoryPut(prop.putId, values)

            prop.onFinish()
            setIsModalOpen(false)
        } catch (error) {

        }
        setConfirmLoading(false)
    }


    // 大专业选择
    const [majorsCategories, setMajorsCategories] = useState<SelectProps['options']>([])
    const [majorsCategoriesValue, setMajorsCategoriesValue] = useState('')
    const [majorsCategoriesSearchValue, setMajorsCategoriesSearchValue] = useState('')
    const onSearchMajorsCategory = async (value: string) => {
        setMajorsCategoriesSearchValue(value)
        const res = await api.MajorsCategory.apiMajorsCategoryGet(1, 9999, value)
        const tmp = res.data.data?.dataList?.map(x => ({ label: x.name, value: x.id }))
        setMajorsCategories(tmp || [])
    };
    const onMajorsChange: SelectProps['onChange'] = (v) => {
        setMajorsCategoriesValue(v)
        setMajorsCategoriesSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
    }
    return (
        <Modal
            title={title}
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
            okText='确定'
            cancelText='取消'
        >

            <Form<MajorsSubcategoryReqDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<MajorsSubcategoryReqDto> name='name' rules={[{ required: true, message: '不能为空' }]} label='专业名'>
                    <Input placeholder="专业名"></Input>
                </Form.Item>
                <Form.Item<MajorsSubcategoryReqDto> name='majorsCategoryId' rules={[{ required: true, message: '不能为空' }]} label='大专业'>
                    <Select
                        showSearch
                        placeholder="输入搜索"
                        onSearch={onSearchMajorsCategory}
                        options={majorsCategories}
                        optionFilterProp="label"
                        value={majorsCategoriesValue}
                        searchValue={majorsCategoriesSearchValue}
                        onChange={onMajorsChange}
                    />
                </Form.Item>
            </Form>
        </Modal>
    )
}