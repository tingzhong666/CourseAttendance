import { ChangeEvent, useEffect, useState } from "react"
import { UserRole } from "../../services/api"
import { Button, DatePicker, Select, SelectProps, Space, TimePicker } from "antd"
import { SearchProps } from "antd/es/input"
import Search from "antd/es/input/Search"
import { useAuth } from "../../Contexts/auth"
import dayjs, { Dayjs } from "dayjs"
import { useMajor } from "../../Contexts/major"
import AttendanceTable from "../../components/AttendanceTable"

const Attendance = () => {
    // 查询参数

    const [current, setCurrent] = useState(1)

    const [queryStr, setQueryStr] = useState('') // 课程名
    const [teacherName, setTeacherName] = useState('') // 老师名
    const [studentName, setStudentName] = useState('') // 学生名
    const [startTime, setStartTime] = useState<dayjs.Dayjs | null>(null) // 时间段
    const [endTime, setEndTime] = useState<dayjs.Dayjs | null>(null) // 时间段
    const [majorsCategoryId, setMajorsCategoryId] = useState<undefined | number>(undefined)
    const [majorsSubcategoriesId, setMajorsSubcategoriesId] = useState<undefined | number>(undefined)

    const auth = useAuth()

    const major = useMajor()

    // 数据更新
    const [dataUpdate, setDataUpdate] = useState(false)



    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        await getData()

        onSearchMajorsCategory('')
        onSearchMajorsCategorySub('')
    }



    const getData = () => {
        setDataUpdate(true)
    }

    // 查询
    const onChangeCourse: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setQueryStr(e.target.value)
    }
    const onSearchCourse: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setQueryStr(value)
        setCurrent(1)
        // await getData(1, undefined, value);
        await getData();
    }
    const onChangeTeacher: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setTeacherName(e.target.value)
    }
    const onSearchTeacher: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setTeacherName(value)
        setCurrent(1)
        // await getData(1, undefined, undefined, undefined, value);
        await getData();
    }
    const onChangeStudent: (event: ChangeEvent<HTMLInputElement>) => void = e => {
        setStudentName(e.target.value)
    }
    const onSearchStudent: SearchProps['onSearch'] = async (value, _e, info) => {
        if (info?.source != 'input') return

        setStudentName(value)
        setCurrent(1)
        // await getData(1, undefined, undefined, value);
        await getData();
    }
    const onChangeStartDate: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setStartTime(null)
            return
        }
        let tmp = startTime?.clone() ?? dayjs()
        tmp = tmp?.year(date.year())
        tmp = tmp?.month(date.month())
        tmp = tmp?.date(date.date())
        setStartTime(tmp || null)
    }
    const onChangeStartTime: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setStartTime(null)
            return
        }
        let tmp = startTime?.clone() ?? dayjs()
        tmp = tmp?.hour(date.hour())
        tmp = tmp?.minute(date.minute())
        tmp = tmp?.second(date.second())
        setStartTime(tmp || null)
    }
    const onChangeEndDate: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setEndTime(null)
            return
        }
        let tmp = endTime?.clone() ?? dayjs()
        tmp = tmp?.year(date.year())
        tmp = tmp?.month(date.month())
        tmp = tmp?.date(date.date())
        setEndTime(tmp || null)
    }
    const onChangeEndTime: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date) {
            setEndTime(null)
            return
        }
        let tmp = endTime?.clone() ?? dayjs()
        tmp = tmp?.hour(date.hour())
        tmp = tmp?.minute(date.minute())
        tmp = tmp?.second(date.second())
        setEndTime(tmp || null)
    }
    const onSearch = async () => {
        setCurrent(1)
        // await getData(1);
        await getData();
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

    return (
        <Space direction='vertical' style={{ width: '100%' }}>

            <Space>
                <Search placeholder="输入查询的课程名" onSearch={onSearchCourse} enterButton onChange={onChangeCourse} />

                {(auth.user?.roles.includes(UserRole.Admin) ||
                    auth.user?.roles.includes(UserRole.Academic) ||
                    auth.user?.roles.includes(UserRole.Student)) &&
                    <Search placeholder="输入查询的老师名" onSearch={onSearchTeacher} enterButton onChange={onChangeTeacher} />
                }

                {(auth.user?.roles.includes(UserRole.Admin) ||
                    auth.user?.roles.includes(UserRole.Academic) ||
                    auth.user?.roles.includes(UserRole.Teacher)) &&
                    <Search placeholder="输入查询的学生名" onSearch={onSearchStudent} enterButton onChange={onChangeStudent} />
                }


                <DatePicker onChange={onChangeStartDate} placeholder='开始日期' />
                <TimePicker onChange={onChangeStartTime} defaultOpenValue={dayjs('00:00:00', 'HH:mm:ss')} />
                -
                <DatePicker onChange={onChangeEndDate} placeholder='结束日期' />
                <TimePicker onChange={onChangeEndTime} defaultOpenValue={dayjs('00:00:00', 'HH:mm:ss')} />



                <Button type='primary' onClick={() => onSearch()}>查询</Button>
            </Space>
            <Space>
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

                {/* 模拟考勤 */}
            </Space>


            <AttendanceTable
                dataUpdate={dataUpdate}
                onDataUpdated={() => setDataUpdate(false)}

                queryStr={queryStr}
                teacherName={teacherName}
                studentName={studentName}
                startTime={startTime}
                endTime={endTime}
                majorsCategoryId={majorsCategoryId}
                majorsSubcategoriesId={majorsSubcategoriesId}

                current={current}
            />
        </Space>
    )
}

export default Attendance