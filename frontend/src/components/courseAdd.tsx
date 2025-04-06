import { Modal, Form, Input, Button, Select, SelectProps } from 'antd'
import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { CourseRequestDto, UserRole } from '../services/api'
import dayjs from 'dayjs'
import TimeQuantumForm, { TimeQuantum } from './TimeQuantumForm'

interface Props {
    show: boolean,
    showChange: (show: boolean) => void
    onFinish: () => void
    model: 'put' | 'add' | 'get'
    putId?: number
}

export type CourseAddProps = Props

interface CourseReqData extends CourseRequestDto {
    // 课时
    timeQuantum: Array<TimeQuantum>
}

export default (prop: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [confirmLoading, setConfirmLoading] = useState(false);
    const [form] = Form.useForm<CourseReqData>();
    const [title, setTitle] = useState('');
    // 初始化
    const init = async () => {
        if (prop.model == 'put' || prop.model == 'get') {
            onSearchTeacher('')
        }
        else if (prop.model == 'add') {
            form.resetFields()
            onSearchTeacher('')
        }


        if (prop.model == 'put')
            setTitle('修改课程')
        else if (prop.model == 'add')
            setTitle('新增课程')
        else if (prop.model == 'get')
            setTitle('课程详情')
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
    };

    const handleOk = () => {
        setIsModalOpen(false);

        form.submit()
    };

    const onFinish = async (values: CourseReqData) => {
        setConfirmLoading(true)

        console.log(values)
        // var res = await api.Course.apiCoursePost(values)

        // if (res.data.code != 1) {
        //     switch (res.data.code) {
        //         case 3:
        //             notification.info({
        //                 message: `请求失败，无效的工号或密码`,
        //                 placement: "topRight",
        //             })
        //             break
        //     }
        //     return;
        // }

        setConfirmLoading(false)
    }


    // 老师选择
    const [teachers, setTeachers] = useState<SelectProps['options']>([])
    const [teacherSelectValue, setTeacherSelectValue] = useState('')
    const [teacherSearchValue, setTeacherSearchValue] = useState('')
    const onSearchTeacher = async (value: string) => {
        setTeacherSearchValue(value)
        const res = await api.Account.apiAccountGet([UserRole.Teacher], 1, 9999, value)
        const tmp = res.data.data?.dataList?.map(x => ({ label: x.name, value: x.id }))
        setTeachers(tmp || [])
    };
    const onChangeTeacher: SelectProps['onChange'] = (v) => {
        setTeacherSelectValue(v)
        setTeacherSearchValue(teachers?.find(x => x.value == v)?.label + '')
    }
    return (
        <Modal
            title={title}
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
            confirmLoading={confirmLoading}
            width={600}
        >
            <Form<CourseReqData>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<CourseReqData> name="name" label='课程名'>
                    <Input placeholder="课程名" />
                </Form.Item>
                <Form.Item<CourseReqData> name="teacherId" label='老师'>
                    <Select
                        showSearch
                        placeholder="输入搜索"
                        onSearch={onSearchTeacher}
                        options={teachers}
                        optionFilterProp="label"
                        value={teacherSelectValue}
                        searchValue={teacherSearchValue}
                        onChange={onChangeTeacher}
                    />
                </Form.Item>

                <Form.Item<CourseReqData> name='timeQuantum' label='课时'>
                    <TimeQuantumForm />
                </Form.Item>
            </Form>
        </Modal>
    )
}