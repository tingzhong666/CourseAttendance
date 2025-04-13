import { DatePicker, TimePicker } from "antd"
import dayjs, { Dayjs } from "dayjs"
import { useEffect, useState } from "react"

interface Props {
    value?: dayjs.Dayjs
    onChange?: (value: dayjs.Dayjs | undefined) => void
    disabled?: boolean
    placeholder?: string
}
export default (props: Props) => {
    const [dateTime, setDateTime] = useState<dayjs.Dayjs | undefined>(dayjs())

    useEffect(() => {
        setDateTime(props.value)
    }, [props.value])
    useEffect(() => {
        if (props.onChange)
            props.onChange(dateTime)
    }, [dateTime])
    const onChangeDate: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date)
            date = dayjs().year(0).month(0).day(0)
        let tmp = dateTime?.clone() ?? dayjs()
        tmp = tmp?.year(date.year())
        tmp = tmp?.month(date.month())
        tmp = tmp?.date(date.date())
        setDateTime(tmp || undefined)
    }
    const onChangeTime: (date: Dayjs, dateString: string | string[]) => void = (date) => {
        if (!date)
            date = dayjs().hour(0).minute(0).second(0)
        if (!date) return
        let tmp = dateTime?.clone() ?? dayjs()
        tmp = tmp?.hour(date.hour())
        tmp = tmp?.minute(date.minute())
        tmp = tmp?.second(date.second())
        setDateTime(tmp || undefined)
    }

    return (
        <>
            <DatePicker onChange={onChangeDate} value={dateTime} placeholder={props.placeholder} disabled={props.disabled} />
            <TimePicker onChange={onChangeTime} value={dateTime} disabled={props.disabled} />
        </>
    )
}