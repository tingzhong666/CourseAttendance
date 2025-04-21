import { useEffect, useState } from "react"
import * as api from '../services/http/httpInstance'
import { Form, Input, InputNumber, Modal, Select, SelectProps } from "antd"
import { GradeRequestDto } from "../services/api"
import { useMajor } from "../Contexts/major"
import YearPicker from "./YearPicker"

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
    model: 'put' | 'add' | 'get'
    putId?: number
}
export type ClassesManagerAddProps = Props;


const ClassesManagerAdd= (prop: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [title, setTitle] = useState('');

    const major = useMajor()


    useEffect(() => {
        setIsModalOpen(prop.show)
        if (prop.show) {
            init()
        }

    }, [prop.show])
    useEffect(() => {
        prop.showChange(isModalOpen)
    }, [isModalOpen])

    // 初始化
    const init = async () => {
        if (prop.model == 'put') {
            var res = await api.Classes.apiClassesIdGet(prop.putId || -1)
            const majorSub = major.majorsSubcategories.find(x => x.id == res.data.data?.majorsSubcategoriesId)
            const toReq: GradeRequestDto = {
                name: res.data.data?.name || '',
                majorsCategoriesId: majorSub?.majorsCategoriesId,
                majorsSubcategoriesId: majorSub?.id,
                num: res.data.data?.num,
                year: res.data.data?.year,
            }

            form.setFieldsValue(toReq)
            setTitle('修改班级')

            // 小专业
            setMajorsCategoriesSub([{
                label: majorSub?.name,
                value: majorSub?.id
            }])
            setMajorsCategoriesSubValue(majorSub?.id || -1)
            setMajorsCategoriesSubSearchValue(majorSub?.name || '')
            // 大专业
            setMajorsCategories([{
                label: majorSub?.parentName,
                value: majorSub?.majorsCategoriesId
            }])
            setMajorsCategoriesValue(majorSub?.majorsCategoriesId || -1)
            setMajorsCategoriesSearchValue(majorSub?.parentName || '')
        }
        else if (prop.model == 'add') {
            form.resetFields()
            setTitle('新增班级')
            onSearchMajorsCategorySub('')
            onSearchMajorsCategory('')
        }
        else if (prop.model == 'get') {
            setTitle('班级详情')
        }

    }



    // 弹框
    const handleCancel = () => {
        setIsModalOpen(false);
    }
    const handleOk = () => {

        form.submit()
    }


    // 表单
    const [form] = Form.useForm<GradeRequestDto>()
    const onFinish = async (values: GradeRequestDto) => {
        try {
            await form.validateFields()
            setConfirmLoading(true)

            if (prop.model == 'add')
                await api.Classes.apiClassesPost(values)
            else if (prop.model == 'put')
                await api.Classes.apiClassesIdPut(prop.putId || -1, values)
            
            setIsModalOpen(false)
            prop.onFinish()
        } catch {
        }
        setConfirmLoading(false)
    }


    // 大专业选择
    const [majorsCategories, setMajorsCategories] = useState<SelectProps['options']>([])
    const [majorsCategoriesValue, setMajorsCategoriesValue] = useState(-1)
    const [majorsCategoriesSearchValue, setMajorsCategoriesSearchValue] = useState('')
    const onSearchMajorsCategory = async (value: string) => {
        setMajorsCategoriesSearchValue(value)
        const res = await api.MajorsCategory.apiMajorsCategoryGet(1, 9999, value)
        const tmp = res.data.data?.dataList?.map(x => ({ label: x.name, value: x.id }))
        setMajorsCategories(tmp || [])

        onSearchMajorsCategorySub('')
    }
    const onMajorsChange: SelectProps['onChange'] = (v) => {
        setMajorsCategoriesValue(v)
        setMajorsCategoriesSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
    }

    // 细分专业选择
    const [majorsCategoriesSub, setMajorsCategoriesSub] = useState<SelectProps['options']>([])
    const [majorsCategoriesSubValue, setMajorsCategoriesSubValue] = useState(-1)
    const [majorsCategoriesSubSearchValue, setMajorsCategoriesSubSearchValue] = useState('')
    const onSearchMajorsCategorySub = async (value: string) => {
        setMajorsCategoriesSubSearchValue(value)
        const res = await api.MajorsSubcategory.apiMajorsSubcategoryGet(majorsCategoriesValue, 1, 9999, value)
        const tmp = res.data.data?.dataList?.map(x => ({ label: x.name, value: x.id }))
        setMajorsCategoriesSub(tmp || [])
    }
    const onMajorsSubChange: SelectProps['onChange'] = (v) => {
        setMajorsCategoriesSubValue(v)
        setMajorsCategoriesSubSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
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

            <Form<GradeRequestDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<GradeRequestDto> name='name'>
                    <Input placeholder="无独立名称可留空"></Input>
                </Form.Item>
                <Form.Item<GradeRequestDto> name='majorsCategoriesId'  rules={[{ required: true, message: '不能为空' }]} label='大专业'>
                    <Select
                        showSearch
                        placeholder="输入搜索大专业名称"
                        onSearch={onSearchMajorsCategory}
                        options={majorsCategories}
                        optionFilterProp="label"
                        value={majorsCategoriesValue}
                        searchValue={majorsCategoriesSearchValue}
                        onChange={onMajorsChange}
                    />
                </Form.Item>
                <Form.Item<GradeRequestDto> name='majorsSubcategoriesId'  rules={[{ required: true, message: '不能为空' }]} label='小专业'>
                    <Select
                        showSearch
                        placeholder="输入搜索细分专业名称"
                        onSearch={onSearchMajorsCategorySub}
                        options={majorsCategoriesSub}
                        optionFilterProp="label"
                        value={majorsCategoriesSubValue}
                        searchValue={majorsCategoriesSubSearchValue}
                        onChange={onMajorsSubChange}
                    />
                </Form.Item>
                <Form.Item<GradeRequestDto> name='year'  rules={[{ required: true, message: '不能为空' }]} label='年级'>
                    <YearPicker DatePickerProps={{ placeholder: "年级" }} />
                </Form.Item>
                <Form.Item<GradeRequestDto> name='num'  rules={[{ required: true, message: '不能为空' }]} label='班级序号'>
                    <InputNumber min={1} placeholder="班级序号" />
                </Form.Item>
            </Form>
        </Modal>
    )
}

export default ClassesManagerAdd