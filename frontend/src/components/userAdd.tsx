import { Form, FormListFieldData, FormListOperation, FormProps, Input, Modal, Select, SelectProps } from 'antd'
import * as api from '../services/http/httpInstance'
import { useEffect, useState } from 'react';
import { CreateUserReqDto, UpdateProfileReqDto, UserRole } from '../services/api';
import { useAuth } from '../Contexts/auth';
import { CreateUUID } from '../Utils/Utils';
import { useMajor } from '../Contexts/major';

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


    const auth = useAuth()


    // 初始化
    const init = async () => {
        if (prop.model == 'put' || prop.model == 'get') {
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

            onSearchClasses('')
            setClassesValue(res.data.data?.getStudentExt?.gradeId || -1)
            setRoles(res.data.data?.roles || [])
        }
        else if (prop.model == 'add') {
            form.resetFields()
            form.setFieldValue('roles', [UserRole.Student])
            onSearchClasses('')
        }

        // 标题
        if (prop.model == 'put')
            setTitle('修改班级')
        else if (prop.model == 'get')
            setTitle('用户详情')
        else if (prop.model == 'add')
            setTitle('新增用户')

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
    const [form] = Form.useForm<CreateUserReqDto>();
    const onFinish = async (values: CreateUserReqDto) => {
        try {

             await form.validateFields()

            setConfirmLoading(true)
            if (prop.model == 'add')
                await api.Account.apiAccountPost(values)
            else if (prop.model == 'put') {
                await api.Account.apiAccountUpdateUserPut(prop.putId, values as UpdateProfileReqDto)
            }

            setIsModalOpen(false);
            prop.onFinish()
        } catch {
        }
        setConfirmLoading(false)
    }


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

    const [roles, setRoles] = useState<Array<UserRole>>([])
    const onChange: FormProps['onValuesChange'] = (cv, v) => {
        setRoles(v.roles)
    }
    return (
        <Modal
            title={title}
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
        >
            <Form<CreateUserReqDto>
                layout="vertical"
                form={form}
                onFinish={onFinish}
                onValuesChange={onChange}
            >
                <Form.Item<CreateUserReqDto> name="name" label="姓名"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Input placeholder="姓名" disabled={prop.model == 'get'} />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="userName" label="工号/学号"
                    rules={[{ required: true, message: '不能为空' }]}
                >
                    <Input placeholder="工号/学号" disabled={prop.model == 'get'} />
                </Form.Item>

                {prop.model == 'add' &&
                    <Form.Item<CreateUserReqDto> name="passWord" label="密码"
                        rules={[{ required: true, message: '不能为空' }]}
                    >
                        <Input.Password placeholder="密码" />
                    </Form.Item>
                }

                <Form.Item<CreateUserReqDto> name="email" label="email"
                >
                    <Input placeholder="email" disabled={prop.model == 'get'} />
                </Form.Item>
                <Form.Item<CreateUserReqDto> name="phone" label="手机号"
                >
                    <Input placeholder="手机号" disabled={prop.model == 'get'} />
                </Form.Item>
                <Form.List name="roles">
                    {
                        (fields: FormListFieldData[], operation: FormListOperation) => (
                            <Form.Item {...fields[0]} key={CreateUUID()} label="身份"
                                rules={[{ required: true, message: '不能为空' }]}
                            >
                                <Select
                                    options={Object.entries(UserRole).map(([, value]) => {
                                        return { label: auth.roleMap(value), value: value }
                                    })}
                                    disabled={prop.model == 'get'}
                                />
                            </Form.Item>
                        )
                    }
                </Form.List>

                {roles.includes(UserRole.Student) &&
                    <Form.Item<CreateUserReqDto> name={['createStudentExt', 'gradId']} label="班级"
                        rules={[{ required: true, message: '不能为空' }]}
                    >
                        <Select
                            showSearch
                            placeholder="输入搜索班级"
                            onSearch={onSearchClasses}
                            options={classes}
                            optionFilterProp="label"
                            value={classesValue}
                            searchValue={classesSearchValue}
                            onChange={onClassesChange}
                            disabled={prop.model == 'get'}
                        />
                    </Form.Item>
                }
            </Form>
        </Modal>
    )
}