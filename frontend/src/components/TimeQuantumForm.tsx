import { Button, DatePicker, DatePickerProps, Flex, Select, SelectProps, Space } from "antd"
import { CloseOutlined, DownloadOutlined, PlusOutlined } from '@ant-design/icons'
import { useEffect, useState } from "react"
import dayjs, { Dayjs } from "dayjs"
import { RangePickerProps } from "antd/es/date-picker"
import 'dayjs/plugin/weekOfYear'
import { NoUndefinedRangeValueType } from "rc-picker/lib/PickerInput/RangePicker"
import { CreateUUID } from "../Utils/Utils"
import * as api from '../services/http/httpInstance'

type WeekDay = '周一' | '周二' | '周三' | '周四' | '周五' | '周六' | '周日'

export interface TimeQuantum {
    // 周几
    day: WeekDay
    // 第几节
    timeTable: number
    // 日期段
    start: dayjs.Dayjs
    end: dayjs.Dayjs

    id: string
}

interface Props {
    value?: Array<TimeQuantum>
    onChange?: (value: Array<TimeQuantum>) => void
}

export default (props: Props) => {
    const [datas, setDatas] = useState<Array<TimeQuantum>>([])

    //初始化
    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        const res = await api.TimeTable.apiTimeTableGet()
        const tmp = res.data.data?.map(x => ({
            label: x.name,
            value: x.id
        }))
        setTimeTables(tmp)
    }

    // 同步
    useEffect(() => {
        if (props.onChange)
            props.onChange(datas)
    }, [datas])
    useEffect(() => {
        if (props.value)
            setDatas(props.value)
    }, [props.value])


    const onAdd = () => {
        const data = {} as TimeQuantum
        data.id = CreateUUID()
        setDatas([...datas, data])
    }

    const onDel = (data: TimeQuantum) => {
        setDatas(datas.filter(x => x.id != data.id))
    }

    const onChangeRangePicker = (date: NoUndefinedRangeValueType<dayjs.Dayjs> | null, data: TimeQuantum) => {
        if (!date) return
        if (!date[0]) return
        if (!date[1]) return

        setDatas(datas.map(x => x.id == data.id ? { ...x, start: dayjs(date[0]), end: dayjs(date[1]) } : x))
    }

    // 周几
    const weekDay: SelectProps['options'] = [
        { value: '周一' },
        { value: '周二' },
        { value: '周三' },
        { value: '周四' },
        { value: '周五' },
        { value: '周六' },
        { value: '周日' },
    ]
    const onChangeWeekDay = (value: WeekDay, data: TimeQuantum) => {
        setDatas(datas.map(x => x.id == data.id ? { ...x, day: value } : x))
    }

    // 第几节
    const [timeTables, setTimeTables] = useState<SelectProps['options']>([])
    const onChangeTimeTable = (value: number, data: TimeQuantum) => {
        setDatas(datas.map(x => x.id == data.id ? { ...x, timeTable: value } : x))
    }

    return (<>
        <Space direction='vertical' style={{ width: '100%' }}>
            {
                datas.map(x => (<Flex key={x.id} gap='small'>
                    <DatePicker.RangePicker onChange={date => onChangeRangePicker(date, x)} picker="week" style={{ flex: 1 }} />

                    <Select
                        placeholder="周几"
                        options={weekDay}
                        onChange={v => onChangeWeekDay(v, x)}
                        showSearch
                        optionFilterProp="label"
                        style={{ width: 100 }}
                    />

                    <Select
                        placeholder="第几节"
                        options={timeTables}
                        onChange={v => onChangeTimeTable(v, x)}
                        showSearch
                        optionFilterProp="label"
                        style={{ width: 100 }}
                    />

                    <Button variant="outlined" color="cyan" block icon={<CloseOutlined />} onClick={() => onDel(x)} style={{ width: 50 }} />

                </Flex>))
            }
            <Button type="default" block icon={<PlusOutlined />} onClick={onAdd} />
        </Space>
    </>)
}