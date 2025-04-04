import { Form, FormListFieldData, FormListOperation, Input, Modal, Select } from 'antd'
import * as api from '../services/http/httpInstance'
import { useEffect, useState } from 'react';
import { CreateUserReqDto, CreateUserStudentReqDto, GetUserResDto, GradeResponseDto, MajorsCategoryResDto, MajorsSubcategoryResDto, UserRole } from '../services/api';
import { BaseOptionType } from 'antd/es/select';
import { useAuth } from '../Contexts/auth';

interface Props {
    show: boolean
    showChange: (show: boolean) => void
}

export default (prop: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [initValueForm, setInitValueForm] = useState({} as CreateUserReqDto);
    const [role, setRole] = useState<UserRole>(UserRole.Student);
    const auth = useAuth()

    const [majorsCategories, setMajorsCategories] = useState<Array<MajorsCategoryResDto>>([])
    const [majorsSubcategories, setMajorsSubcategories] = useState<Array<MajorsSubcategoryResDto>>([])
    const [classes, setClasses] = useState<Array<GradeResponseDto>>([])
    const [classesStr, setClassesStr] = useState<Array<string>>([])

    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        Promise.all([
            getMajorsCategories(),
            getMajorsSubcategories(),
            getClasses(),
        ])

        const tmp = classes.map(x => {
            const majorsSubcategory = majorsSubcategories.find(v => v.id == x.majorsSubcategoriesId)
            const majorsCategory = majorsCategories.find(v => v.id == majorsSubcategory?.majorsCategoriesId)

            return `${x.year}级-${majorsCategory?.name}-${majorsSubcategory?.name}-${x.num}班`
        })

        setClassesStr(tmp)

        console.log(classesStr)
    }

    const getClasses = async () => {
        const res = await api.Classes.apiClassesGet()
        setClasses(res.data.data ?? [])

    }
    const getMajorsSubcategories = async () => {
        const res = await api.MajorsSubcategory.apiMajorsSubcategoryGet(1, 9999)
        setMajorsSubcategories(res.data.data?.dataList ?? [])

    }
    const getMajorsCategories = async () => {
        const res = await api.MajorsCategory.apiMajorsCategoryGet(1, 9999)
        setMajorsCategories(res.data.data?.dataList ?? [])

    }

    const [form] = Form.useForm();
    useEffect(() => {
        setIsModalOpen(prop.show)

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

    const onFinish = async (values: CreateUserReqDto) => {
        setConfirmLoading(true)

        var res = await api.Account.apiAccountPost(values)
        setConfirmLoading(false)
    }

    const ExtForm = () => {
        const [form] = Form.useForm();

        switch (role) {
            case UserRole.Admin:
            case UserRole.Academic:
            case UserRole.Teacher:
                return <></>
            case UserRole.Student:
                return (
                    <Form
                        layout="vertical"
                        form={form}
                    >
                        {/* <Form.Item<CreateUserStudentReqDto> name="gradId" label="班级">
                            <Select
                                defaultValue={UserRole.Student}
                                options={Object.entries(UserRole).map(([, value]) => {
                                    return { label: auth.roleMap(value), value: value }
                                })}
                            />
                        </Form.Item> */}
                    </Form>)
        }
    }

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
                initialValues={initValueForm}
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
                                <Form.Item {...fields[0]}>
                                    <Select
                                        defaultValue={UserRole.Student}
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