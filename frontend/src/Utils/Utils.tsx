import { v4 } from "uuid"
import { WeekDay } from "../models/WeekDay"
import jsQR, { QRCode } from "jsqr"

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

const loadImage = (url: string): Promise<HTMLImageElement> => {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.src = url;
        img.onload = () => resolve(img);
        img.onerror = (error) => reject(error);
    });
};

export const QRDeCode = async (file: Blob | MediaSource): Promise<string> => {
    const url = URL.createObjectURL(file)
    const img = await loadImage(url)

    const canvas = document.createElement('canvas')
    const ctx = canvas.getContext('2d')

    if (!ctx) return ''

    canvas.width = img.width
    canvas.height = img.height
    ctx.drawImage(img, 0, 0)

    // 获取 ImageData
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)
    const res = jsQR(imageData.data, canvas.width, canvas.height);

    URL.revokeObjectURL(url)
    return res?.data || ''
}