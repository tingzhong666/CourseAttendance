import { Form, FormListFieldData, FormListOperation, Input, Modal, Select } from 'antd'
import * as api from '../services/http/httpInstance'
import { useEffect, useState } from 'react';
import { CreateUserReqDto, CreateUserStudentReqDto, UpdateProfileReqDto, UserRole } from '../services/api';
import { SelectProps } from 'antd/es/select';
import { useAuth } from '../Contexts/auth';
import { useMajor } from '../Contexts/major';
import { CreateUUID } from '../Utils/Utils';

interface Props {
    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
    model: 'put' | 'add' | 'get'
    putId?: string
}
export type UserAddProps = Props;

export default (prop: Props) => {
    // 弹框数据
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [title, setTitle] = useState('');


    const [role, setRole] = useState<UserRole>(UserRole.Student);
    const auth = useAuth()


    // 初始化
    const init = async () => {
        if (prop.model == 'put') {
            var res = await api.Account.apiAccountIdGet(prop.putId || '')
            const toReq: UpdateProfileReqDto = {
                email: res.data.data?.email,
                phone: res.data.data?.phoneNumber,
                name: res.data.data?.name || '',
                userName: res.data.data?.userName || '',
                // passWord: '',
                roles: res.data.data?.roles || [],
                createAcademicExt: {},
                createAdminExt: {},
                createStudentExt: {
                    gradId: res.data.data?.getStudentExt?.gradeId || -1
                },
                createTeacherExt: {},

            }

            form.setFieldsValue(toReq)
            setTitle('修改班级')

            // 小专业
            // setMajorsCategoriesSub([{
            //     label: majorSub?.name,
            //     value: majorSub?.id
            // }])
            // setMajorsCategoriesSubValue(majorSub?.id || -1)
            // setMajorsCategoriesSubSearchValue(majorSub?.name || '')
            // // 大专业
            // setMajorsCategories([{
            //     label: majorSub?.parentName,
            //     value: majorSub?.majorsCategoriesId
            // }])
            // setMajorsCategoriesValue(majorSub?.majorsCategoriesId || -1)
            // setMajorsCategoriesSearchValue(majorSub?.parentName || '')
        }
        else if (prop.model == 'add') {
            form.resetFields()
            setTitle('新增用户')
            // onSearchClasses('')
        }
        else if (prop.model == 'get') {
            setTitle('用户详情')
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
        setIsModalOpen(false);

        form.submit()
    }

    // 表单
    const [form] = Form.useForm<CreateUserReqDto>();
    const onFinish = async (values: CreateUserReqDto) => {
        try {
            setConfirmLoading(true)
            if (prop.model == 'add')
                await api.Account.apiAccountPost(values)
            else if (prop.model == 'put')
                await api.Account.apiAccountUpdateUserPut(prop.putId, values as UpdateProfileReqDto)

        } catch {
        }
        setConfirmLoading(false)
    }




    // 不同身份的扩展信息
    const [form2] = Form.useForm();
    const ExtForm = () => {
        // 班级选择
        const major = useMajor()
        const [classes, setClasses] = useState<SelectProps['options']>([])
        const [classesValue, setClassesValue] = useState(-1)
        const [classesSearchValue, setClassesSearchValue] = useState('')
        const onSearchClasses = (value: string) => {
            setClassesSearchValue(value)
            const tmp = major.classesStr.map(x => ({ label: x.name, value: x.id }))
            setClasses(tmp || [])

        }
        const onClassesChange: SelectProps['onChange'] = (v) => {
            setClassesValue(v)
            setClassesSearchValue(classes?.find(x => x.value == v)?.label + '')

        }

        switch (role) {
            case UserRole.Admin:
            case UserRole.Academic:
            case UserRole.Teacher:
                return <></>
            case UserRole.Student:
                return (
                    <Form
                        layout="vertical"
                        form={form2}
                    >
                        <Form.Item<CreateUserStudentReqDto> name="gradId" label="班级">
                            <Select
                                showSearch
                                placeholder="输入搜索班级"
                                onSearch={onSearchClasses}
                                options={classes}
                                optionFilterProp="label"
                                value={classesValue}
                                searchValue={classesSearchValue}
                                onChange={onClassesChange}
                            />
                        </Form.Item>
                    </Form>
                )
            default:
                return <></>

        }
    }

    return (
        <Modal
            title={title}
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
                <Form.Item<CreateUserReqDto> name="name">
                    <Input placeholder="姓名" />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="userName">
                    <Input placeholder="工号/学号" />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="passWord">
                    <Input.Password placeholder="密码" />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="email">
                    <Input placeholder="email" />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="phone">
                    <Input placeholder="手机号" />
                </Form.Item>
                <Form.List name="roles">
                    {
                        (fields: FormListFieldData[], operation: FormListOperation) => {

                            if (fields.length != 1) operation.add()


                            const handleChange = (value: UserRole) => {
                                setRole(value)
                            }

                            return (
                                <Form.Item {...fields[0]} key={CreateUUID()}>
                                    <Select
                                        // defaultValue={UserRole.Student}
                                        onChange={handleChange}
                                        options={Object.entries(UserRole).map(([, value]) => {
                                            return { label: auth.roleMap(value), value: value }
                                        })}
                                    />
                                </Form.Item>
                            )
                        }
                    }
                </Form.List>
            </Form>
            <ExtForm />
        </Modal>
    )
}