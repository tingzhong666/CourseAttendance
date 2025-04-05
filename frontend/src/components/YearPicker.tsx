import { DatePicker, DatePickerProps } from "antd"
import { useEffect, useState } from "react"
import dayjs from 'dayjs'

interface Props {
    value?: number,
    onChange?: (value: number) => void,
    DatePickerProps?: DatePickerProps
}
export default (prop: Props) => {

    const [value, setValue] = useState<DatePickerProps['value']>()

    useEffect(() => {
        if (!prop.value) return
        setValue(dayjs().year(prop.value))
    }, [prop.value])

    const onChange: DatePickerProps['onChange'] = (date, dateStr) => {
        if (!prop.onChange) return
        prop.onChange(Number(dateStr))
    }
    return (<>
        <DatePicker
            {...prop.DatePickerProps}
            picker="year"
            onChange={onChange}
            value={value}
        />
    </>)
}