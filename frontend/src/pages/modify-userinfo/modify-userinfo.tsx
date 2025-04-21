import { Button, Form, Input, Select, SelectProps } from "antd"
import * as api from '../../services/http/httpInstance'
import { UpdateProfileReqDto, UpdateProfileSelfReqDto, UserRole } from "../../services/api";
import { useEffect, useState } from "react";
import { useMajor } from "../../Contexts/major";

const ModifyUserinfo = () => {


    useEffect(() => {
        init()
    }, [])


    // 初始化
    const init = async () => {
        var res = await api.Account.apiAccountProfileSelfGet()
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


    // 表单
    const [form] = Form.useForm<UpdateProfileSelfReqDto>();
    const onFinish = async (values: UpdateProfileSelfReqDto) => {
        try {
            console.log(values)
            await api.Account.apiAccountUpdateUserSelfPut(values)
        } catch {
        }
    }

    // 身份与扩展表单信息
    const [roles, setRoles] = useState<Array<UserRole>>([])


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
    useEffect(() => {
        onSearchClasses('')
    }, [major.classesStr])

    return (<>
        <Form<UpdateProfileSelfReqDto>
            layout="vertical"
            form={form}
            onFinish={onFinish}
        >
            <Form.Item<UpdateProfileSelfReqDto> name="name" label="姓名"
                rules={[{ required: true, message: '不能为空' }]}
            >
                <Input placeholder="姓名" />
            </Form.Item>
            <Form.Item<UpdateProfileSelfReqDto> name="userName" label="工号/学号"
                rules={[{ required: true, message: '不能为空' }]}
            >
                <Input placeholder="工号/学号" />
            </Form.Item>

            <Form.Item<UpdateProfileSelfReqDto> name="email" label="email"
            >
                <Input placeholder="email" />
            </Form.Item>
            <Form.Item<UpdateProfileSelfReqDto> name="phone" label="手机号"
            >
                <Input placeholder="手机号" />
            </Form.Item>

            {roles?.includes(UserRole.Student) &&
                <Form.Item<UpdateProfileSelfReqDto> name={['createStudentExt', 'gradId']} label="班级"
                    rules={[{ required: true, message: '不能为空' }]}
                    style={{ display: 'none' }}
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
                    />
                </Form.Item>
            }
            <Button htmlType="submit" type="primary">确认</Button>
        </Form>
    </>)
}

export default ModifyUserinfo