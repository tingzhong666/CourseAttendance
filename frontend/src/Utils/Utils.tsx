import { v4 } from "uuid"
// import { Weekday } from "../services/api"

// export const WeekdayMap = (weekday: Weekday | undefined | null): string => {
//     switch (weekday) {
//         case Weekday.Monday:
//             return '周一'
//         case Weekday.Tuesday:
//             return '周二'
//         case Weekday.Wednesday:
//             return '周三'
//         case Weekday.Thursday:
//             return '周四'
//         case Weekday.Friday:
//             return '周五'
//         case Weekday.Saturday:
//             return '周六'
//         case Weekday.Sunday:
//             return '周日'
//         default:
//             return ''
//     }
// }

export const CreateUUID = (): string => {
    return v4()
}