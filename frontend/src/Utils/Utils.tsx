import { v4 } from "uuid"
import { WeekDay } from "../models/WeekDay"

export const WeekdayToString = (weekday: WeekDay): WeekDay => {
    switch (weekday) {
        case 0:
            return '周一'
        case 1:
            return '周二'
        case 2:
            return '周三'
        case 3:
            return '周四'
        case 4:
            return '周五'
        case 5:
            return '周六'
        case 6:
            return '周日'
        default:
            return weekday
    }
}

export const CreateUUID = (): string => {
    return v4()
}