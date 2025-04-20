import { useEffect, useState } from 'react'
import * as api from '../../services/http/httpInstance'
import { Button, PaginationProps, Popconfirm, Select, SelectProps, Space, Table } from 'antd';
import { ColumnsType } from 'antd/es/table';
import { CourseResponseDto, CourseTimeResDto, UserRole } from '../../services/api';
import Search, { SearchProps } from 'antd/es/input/Search';
import { useAuth } from '../../Contexts/auth';
import { CreateUUID, WeekdayToString, } from '../../Utils/Utils';
import CourseAdd, { CourseAddProps } from '../../components/courseAdd';
import { WeekDay } from '../../models/WeekDay';
import lodash from 'lodash'
import dayjs from 'dayjs';
import { useLocation } from 'react-router';
import { useMajor } from '../../Contexts/major';

interface CourseTimeData {
    // 周几
    weekday: WeekDay,
    // 第几节
    section: string,
    // 开始周
    startWeek: dayjs.Dayjs,
    // 结束周
    endWeek: dayjs.Dayjs,
    key: string
}

interface CourseData extends CourseResponseDto {
    // 是否选了此课 学生身份用
    isSelected: boolean | null,
    // 是否是我亲自授课 老师身份用
    isTeach: boolean | null,
    // 授课老师的名字
    taecherName: string,
    // 上课时间
    timeDatas: Array<CourseTimeData>,
}

export default () => {
    const auth = useAuth()
    const [putId, setPutId] = useState(-1)
    const [addModel, setAddModel] = useState<CourseAddProps['model']>('add')

    // 查询参数
    const [data, setData] = useState([] as Array<CourseData>)
    const [total, setTotal] = useState(0)
    const [current, setCurrent] = useState(1)
    const [limit, setLimit] = useState(20)
    const [queryStr, setQueryStr] = useState('')
    const [majorsCategoryId, setMajorsCategoryId] = useState<undefined | number>(undefined)
    const [majorsSubcategoriesId, setMajorsSubcategoriesId] = useState<undefined | number>(undefined)

    const [addShow, setAddShow] = useState(false)

    const location = useLocation()

    const major = useMajor()


    useEffect(() => {
        init()
    }, []);

    const init = async () => {
        await getData()

        onSearchMajorsCategory('')
        onSearchMajorsCategorySub('')
    }

    // 名 老师 地点 时间(每周哪几节节课上) 操作：选课 退课 删除
    const columns: ColumnsType<CourseData> = [
        {
            title: '课程名',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: '老师',
            dataIndex: 'taecherName',
            key: 'teacher',
        },
        {
            title: '地点',
            dataIndex: 'location',
            key: 'location',
        },
        {
            title: '时间',
            key: 'time',
            //dataIndex: 'tags',
            render: (_, record) => {
                // const grp = lodash.groupBy(record.timeDatas, x => x.weekday)
                // const renders = lodash.map(grp, value => {
                //     const v2 = value.sort((a, b) => a.startWeek < b.startWeek ? -1 : 1)
                //     return (
                //         <div key={CreateUUID()}>
                //             {lodash.first(v2)?.weekday}({lodash.first(v2)?.startWeek.year()}年{lodash.first(v2)?.startWeek.week()}-{lodash.last(v2)?.endWeek.year()}年{lodash.last(v2)?.endWeek.week()}周)-{lodash.first(v2)?.section}
                //         </div>)
                // })
                const renders = record.timeDatas.map(x => {
                    return (
                        <div key={x.key}>
                            {x.weekday}({x.startWeek.year()}年{x.startWeek.week()}-{x.endWeek.year()}年{x.endWeek.week()}周)-{x.section}
                        </div>)
                })
                return (<>
                    {renders}
                </>)
            }
        },
        {
            title: '操作',
            key: 'action',
            render: (_, record) => {

                // 删改
                const isDel = () => {
                    if (auth.user?.roles.includes('Admin'))
                        return true
                    if (auth.user?.roles.includes('Academic'))
                        return true
                    // if (auth.user?.roles.includes('Teacher') && record.isTeach)
                    //     return true
                    return false
                }


                return (
                    <Space size="middle">
                        {auth.user?.roles.includes('Student') && !record.isSelected ? <a onClick={() => selectCourse(record.id)}>选课</a> : <></>}
                        {auth.user?.roles.includes('Student') && record.isSelected ? <a onClick={() => dropCourse(record.id)}>退课</a> : <></>}

                        <a onClick={() => get(record.id || -1)}>查看</a>
                        {isDel() && <a onClick={() => put(record.id || -1)}>修改</a>}
                        {isDel() &&
                            <Popconfirm
                                title="提示"
                                description={`确定删除课程 ${record.name} ?`}
                                onConfirm={() => del(record)}
                                okText="确定"
                                cancelText="取消"
                            >
                                <Button danger>删除</Button>
                            </Popconfirm>
                        }
                    </Space>
                )
            },
        },
    ]

    // 选课
    const selectCourse = async (courseId: number | undefined) => {
        await api.CourseSelection.apiCourseSelectionAddSelfGet(courseId)
        await getData()
    }
    // 退课
    const dropCourse = async (courseId: number | undefined) => {
        await api.CourseSelection.apiCourseSelectionDelSelfGet(courseId)
        await getData()
    }

    // 获取课程列表
    const getData = async (current_ = current, limit_ = limit, queryStr_ = queryStr, majorsCategoryId_ = majorsCategoryId, majorsSubcategoriesId_ = majorsSubcategoriesId) => {
        let studentIds: string[] = []
        let teacherIds: string[] = []
        if (location.pathname == '/my-courses') {
            studentIds = auth.user?.roles.includes(UserRole.Student) ? [auth.user.id] : []
            teacherIds = auth.user?.roles.includes(UserRole.Teacher) ? [auth.user.id] : []
        }
        const res = await api.Course.apiCourseGet([...studentIds], teacherIds, majorsCategoryId_, majorsSubcategoriesId_, current_ - 1, limit_, queryStr_)



        const tmp = res.data.data?.dataList?.map(async x => {
            const cd = x as CourseData
            // 授课老师名字
            const res2 = await api.Account.apiAccountIdGet(cd.teacherId)
            cd.taecherName = res2.data.data?.name || ''
            // 是否选了此课 学生身份用
            cd.isSelected = cd.studentIds.includes(auth.user?.id || '')
            // 是否是我亲自授课 老师身份用
            cd.isTeach = cd.teacherId == auth.user?.id
            // 课程时间
            cd.timeDatas = await timeDataConvert(cd.courseTimes || [])

            // const asd = lodash.groupBy(cd.courseTimes, x => dayjs(x.dateDay).day())
            // // console.log(lodash.keys(x))

            // lodash.map(lodash.keys(asd), x => {
            //     console.log(x)
            // })

            return cd
        }) ?? []


        setData(await Promise.all(tmp) || [])
        setTotal(res.data.data?.total || 0)
    }

    // 课程时间转换
    const timeDataConvert = async (dto: CourseTimeResDto[]): Promise<CourseTimeData[]> => {
        // const tmp = dto.map(async v => {
        //     const res = await api.TimeTable.apiTimeTableIdGet(v.timeTableId || -1)
        //     return {
        //         weekday: WeekdayToString(dayjs(v.dateDay || '').day()),
        //         section: res.data.data?.name,
        //         key: CreateUUID(),
        //         startWeek: dayjs(v.dateDay || ''),
        //         endWeek: dayjs(v.dateDay || ''),
        //     } as CourseTimeData
        // }) ?? []
        const tmp = dto.map(async v => {
            const res = await api.TimeTable.apiTimeTableIdGet(v.timeTableId || -1)
            return {
                section: res.data.data?.name || '',
                ...v
            }
        }) ?? []

        const tmp2 = await Promise.all(tmp)

        const tmp3 = lodash.groupBy(tmp2, x => dayjs(x.dateDay || '').day() + x.section)
        const tmp4: Array<CourseTimeData> = lodash.map(tmp3, x => {
            const x2 = x.sort((a, b) => dayjs(a.dateDay || '').week() < dayjs(b.dateDay || '').week() ? -1 : 1)
            return {
                weekday: WeekdayToString(dayjs(x[0].dateDay || '').day()),
                section: x[0].section,
                key: CreateUUID(),
                startWeek: dayjs(x2[0].dateDay || ''),
                endWeek: dayjs(lodash.last(x2)?.dateDay || ''),
            } as CourseTimeData
        })

        return tmp4
    }

    // 查询
    const onSearch: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setQueryStr(value)
        setCurrent(1)
        await getData(1, undefined, value);
    }

    // 页码及分页大小变化
    const onPageChange: PaginationProps['onChange'] = async (page, pageSize) => {
        setCurrent(page)
        setLimit(pageSize)


        await getData(page, pageSize);
    }

    // 条目操作
    const add = async () => {
        setAddModel('add')
        setAddShow(true)
    }
    const del = async (v: CourseData) => {
        await api.Course.apiCourseIdDelete(v.id || -1)
        await getData()
    }
    const put = async (id: number) => {
        setAddModel('put')
        setAddShow(true)
        setPutId(id)
    }
    const get = async (id: number) => {
        setAddModel('get')
        setAddShow(true)
        setPutId(id)
    }


    // 大专业选择
    const [majorsCategories, setMajorsCategories] = useState<SelectProps['options']>([])
    const [majorsCategoriesSearchValue, setMajorsCategoriesSearchValue] = useState('')
    const onSearchMajorsCategory = async (value: string) => {
        setMajorsCategoriesSearchValue(value)
        const tmp = major.majorsCategories.map(x => ({ label: x.name, value: x.id }))
        setMajorsCategories(tmp || [])

    }
    const onMajorsChange: SelectProps['onChange'] = (v) => {
        setMajorsCategoryId(v)
        setMajorsCategoriesSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
        onSearchMajorsCategorySub('', v)
    }

    // 细分专业选择
    const [majorsCategoriesSub, setMajorsCategoriesSub] = useState<SelectProps['options']>([])
    const [majorsCategoriesSubSearchValue, setMajorsCategoriesSubSearchValue] = useState('')
    const onSearchMajorsCategorySub = async (value: string, majorsCategoryId_ = majorsCategoryId) => {
        setMajorsCategoriesSubSearchValue(value)
        if (!majorsCategoryId_) return
        const tmp = major.majorsSubcategories.filter(x => x.majorsCategoriesId == majorsCategoryId_).map(x => ({ label: x.name, value: x.id }))
        setMajorsCategoriesSub(tmp || [])
    }
    const onMajorsSubChange: SelectProps['onChange'] = (v) => {
        setMajorsSubcategoriesId(v)
        setMajorsCategoriesSubSearchValue(majorsCategories?.find(x => x.value == v)?.label + '')
    }

    return (<Space direction='vertical' style={{ width: '100%' }}>
        <Space>
            <Search placeholder="输入查询的课程名" onSearch={onSearch} enterButton />

            <Select
                showSearch
                placeholder="输入搜索大专业名称"
                options={majorsCategories}
                value={majorsCategoryId}
                onChange={onMajorsChange}

                onSearch={onSearchMajorsCategory}
                searchValue={majorsCategoriesSearchValue}
                optionFilterProp="label"
            />
            <Select
                showSearch
                placeholder="输入搜索细分专业名称"
                options={majorsCategoriesSub}
                value={majorsSubcategoriesId}
                onChange={onMajorsSubChange}

                optionFilterProp="label"
                onSearch={onSearchMajorsCategorySub}
                searchValue={majorsCategoriesSubSearchValue}
            />

            {/* 管理员 教务处 老师 可以新增课程 */}
            {auth.user?.roles.includes('Admin') ||
                auth.user?.roles.includes('Academic') ?
                <Button type='primary' onClick={() => add()}>新增</Button>
                : <></>
            }
        </Space>

        <Table<CourseData>
            columns={columns}
            pagination={{ position: ['bottomCenter'], total, showSizeChanger: true, current, pageSize: limit, onChange: onPageChange }}
            dataSource={data}
            rowKey="id"
        />

        <CourseAdd show={addShow} showChange={x => setAddShow(x)} onFinish={getData} model={addModel} putId={putId} />
    </Space>)
}