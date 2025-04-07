import { Modal, Form, Input, Button, Select, SelectProps } from 'antd'
import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { CourseRequestDto, CourseTimeReqDto, TimeTableResDto, UserRole } from '../services/api'
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

        const res = await api.TimeTable.apiTimeTableGet()
        setTimeTables(res.data.data || [])
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

        form.submit()
    };

    // 课时
    const [timeTables, setTimeTables] = useState<Array<TimeTableResDto>>([])


    const onFinish = async (values: CourseReqData) => {
        try {
            setConfirmLoading(true)
            values.courseTimes = values.timeQuantum.flatMap(x => {
                // 获得时间段内的所有周几
                let current = x.start.startOf('week').add(0, 'day');
                switch (x.day) {
                    case '周一':
                        current = current.add(0, 'day');
                        break;
                    case '周二':
                        current = current.add(1, 'day');
                        break;
                    case '周三':
                        current = current.add(2, 'day');
                        break;
                    case '周四':
                        current = current.add(3, 'day');
                        break;
                    case '周五':
                        current = current.add(4, 'day');
                        break;
                    case '周六':
                        current = current.add(5, 'day');
                        break;
                    case '周日':
                        current = current.add(6, 'day');
                        break;
                }

                const allDays: dayjs.Dayjs[] = []
                while (current.isBefore(x.end.startOf('week').add(6, 'day')) || current.isSame(x.end.startOf('week').add(6, 'day'))) {
                    allDays.push(current);
                    current = current.add(1, 'week') // 每次加一周
                }

                // 拼接当天课时
                let allDays2: CourseTimeReqDto[] = allDays.map(v => {
                    return {
                        dateDay: v.toString(),
                        timeTableId: x.timeTable,
                        // courseId 要课程创建后才有，交给服务端
                        //courseId: -1
                    }
                })
                return allDays2
            })
            //console.log(values)
            var res = await api.Course.apiCoursePost(values)

            prop.onFinish()
            setIsModalOpen(false);
        } catch (e) {
        }
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


                <Form.Item<CourseReqData> name="location" label='位置地点'>
                    <Input placeholder="位置地点" />
                </Form.Item>
            </Form>
        </Modal>
    )
}