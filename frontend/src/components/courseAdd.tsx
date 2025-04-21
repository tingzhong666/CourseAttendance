import { Modal, Form, Input, Select, SelectProps } from 'antd'
import { useEffect, useState } from 'react'
import * as api from '../services/http/httpInstance'
import { CourseRequestDto, CourseTimeReqDto, UserRole } from '../services/api'
import dayjs from 'dayjs'
import TimeQuantumForm, { TimeQuantum } from './TimeQuantumForm'
import { groupBy, map } from 'lodash'
import { CreateUUID, WeekdayToString } from '../Utils/Utils'
import { useMajor } from '../Contexts/major'

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
    // 大专业
    majorsCategoriesId: number,


}

const CourseAdd = (prop: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false)
    const [confirmLoading, setConfirmLoading] = useState(false)
    const [form] = Form.useForm<CourseReqData>()
    const [title, setTitle] = useState('')

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
        if (prop.model == 'put' || prop.model == 'get') {
            onSearchTeacher('')

            var res = await api.Course.apiCourseIdGet(prop.putId || -1)

            // const group = lodash.groupBy(res.data.data?.courseTimes, x => dayjs(x.dateDay).day())

            // const timeQuantum = lodash.map(group, x => {
            //     const sort = x.sort(v => dayjs(v.dateDay).week())
            //     const res2: TimeQuantum = {
            //         day: WeekdayToString(dayjs(lodash.first(sort)?.dateDay).day()),
            //         timeTable: lodash.first(sort)?.timeTableId || -1,
            //         start: dayjs(lodash.first(sort)?.dateDay),
            //         end: dayjs(lodash.last(sort)?.dateDay),
            //         id: CreateUUID()
            //     }
            //     return res2
            // })

            const tmp = res.data.data?.courseTimes?.map(async v => {
                const res = await api.TimeTable.apiTimeTableIdGet(v.timeTableId || -1)
                return {
                    section: res.data.data?.id || -1,
                    ...v
                }
            }) ?? []

            const tmp2 = await Promise.all(tmp)

            const tmp3 = groupBy(tmp2, x => dayjs(x.dateDay || '').day() + x.section)
            const timeQuantum: Array<TimeQuantum> = map(tmp3, x => {
                const x2 = x.sort((a, b) => dayjs(a.dateDay || '').week() < dayjs(b.dateDay || '').week() ? -1 : 1)
                return {
                    day: WeekdayToString(dayjs(x[0].dateDay || '').day()),
                    timeTable: x[0].section,
                    start: dayjs(x2[0].dateDay || ''),
                    end: dayjs(x2[0].dateDay || ''),
                    id: CreateUUID(),
                } as TimeQuantum
            })


            const majorsCategoriesId = major.majorsSubcategories.find(x => x.id == res.data.data?.majorsSubcategoryId)?.majorsCategoriesId ?? -1
            const toReq: CourseReqData = {
                name: res.data.data?.name || '',
                courseTimes: res.data.data?.courseTimes || [],
                location: res.data.data?.location || '',
                teacherId: res.data.data?.teacherId || '',
                timeQuantum,
                majorsSubcategoryId: res.data.data?.majorsSubcategoryId || -1,
                majorsCategoriesId,

            }

            form.setFieldsValue(toReq)
        }
        else if (prop.model == 'add') {
            form.resetFields()
            onSearchTeacher('')
        }
        onSearchMajorsCategory('')
        onSearchMajorsCategorySub('')

        if (prop.model == 'put')
            setTitle('修改课程')
        else if (prop.model == 'add')
            setTitle('新增课程')
        else if (prop.model == 'get')
            setTitle('课程详情')

        //const res = await api.TimeTable.apiTimeTableGet()
        //setTimeTables(res.data.data || [])
    }



    const handleCancel = () => {
        setIsModalOpen(false);
    };

    const handleOk = () => {

        form.submit()
    };

    // 课时
    //const [timeTables, setTimeTables] = useState<Array<TimeTableResDto>>([])


    const onFinish = async (values: CourseReqData) => {
        try {
            await form.validateFields()
            setConfirmLoading(true)

            if (prop.model == 'add') {

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


                // if (auth.user?.roles.includes(UserRole.Teacher)) {
                //     values.teacherId = auth.user?.id
                // }

                await api.Course.apiCoursePost(values)
            }
            else if (prop.model == 'put') {

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
                            courseId: prop.putId,
                        }
                    })
                    return allDays2
                })
                await api.Course.apiCourseIdPut(prop.putId || -1, values)
            }
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

    // 大专业选择
    const [majorsCategories, setMajorsCategories] = useState<SelectProps['options']>([])
    const [majorsCategoriesValue, setMajorsCategoriesValue] = useState(-1)
    const [majorsCategoriesSearchValue, setMajorsCategoriesSearchValue] = useState('')
    const onSearchMajorsCategory = async (value: string) => {
        setMajorsCategoriesSearchValue(value)
        const res = await api.MajorsCategory.apiMajorsCategoryGet(1, 9999, value)
        const tmp = res.data.data?.dataList?.map(x => ({ label: x.name, value: x.id }))
        setMajorsCategories(tmp || [])

    }
    const onMajorsChange: SelectProps['onChange'] = (v) => {
        setMajorsCategoriesValue(v)
        setMajorsCategoriesSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
        onSearchMajorsCategorySub('')
    }

    // 细分专业选择
    const [majorsCategoriesSub, setMajorsCategoriesSub] = useState<SelectProps['options']>([])
    const [majorsCategoriesSubValue, setMajorsCategoriesSubValue] = useState<Number | undefined>(-1)
    const [majorsCategoriesSubSearchValue, setMajorsCategoriesSubSearchValue] = useState('')
    const onSearchMajorsCategorySub = async (value: string) => {
        setMajorsCategoriesSubSearchValue(value)
        const res = await api.MajorsSubcategory.apiMajorsSubcategoryGet(form.getFieldValue('majorsCategoriesId'), 1, 9999, value)
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
            width={600}
        >
            <Form<CourseReqData>
                layout="vertical"
                form={form}
                onFinish={onFinish}
            >
                <Form.Item<CourseReqData> name="name" label='课程名' rules={[{ required: true, message: '不能为空' }]}>
                    <Input placeholder="课程名" disabled={prop.model == 'get'} />
                </Form.Item>

                <Form.Item<CourseReqData> name="teacherId" label='老师' rules={[{ required: true, message: '不能为空' }]}>
                    <Select
                        showSearch
                        placeholder="输入搜索"
                        onSearch={onSearchTeacher}
                        options={teachers}
                        optionFilterProp="label"
                        value={teacherSelectValue}
                        searchValue={teacherSearchValue}
                        onChange={onChangeTeacher}
                        disabled={prop.model == 'get'}
                    />
                </Form.Item>

                <Form.Item<CourseReqData> name='majorsCategoriesId' label='大专业' rules={[{ required: true, message: '不能为空' }]}>
                    <Select
                        showSearch
                        placeholder="输入搜索大专业名称"
                        onSearch={onSearchMajorsCategory}
                        options={majorsCategories}
                        optionFilterProp="label"
                        value={majorsCategoriesValue}
                        searchValue={majorsCategoriesSearchValue}
                        onChange={onMajorsChange}
                        disabled={prop.model == 'get'}
                    />
                </Form.Item>
                <Form.Item<CourseReqData> name='majorsSubcategoryId' label='专业' rules={[{ required: true, message: '不能为空' }]}>
                    <Select
                        showSearch
                        placeholder="输入搜索细分专业名称"
                        onSearch={onSearchMajorsCategorySub}
                        options={majorsCategoriesSub}
                        optionFilterProp="label"
                        value={majorsCategoriesSubValue}
                        searchValue={majorsCategoriesSubSearchValue}
                        onChange={onMajorsSubChange}
                        disabled={prop.model == 'get'}
                    />
                </Form.Item>

                <Form.Item<CourseReqData> name='timeQuantum' label='课时' rules={[{ required: true, message: '不能为空' }]}>
                    <TimeQuantumForm disabled={prop.model == 'get'} />
                </Form.Item>


                <Form.Item<CourseReqData> name="location" label='位置地点' rules={[{ required: true, message: '不能为空' }]}>
                    <Input placeholder="位置地点" disabled={prop.model == 'get'} />
                </Form.Item>
            </Form>
        </Modal>
    )
}

export default CourseAdd