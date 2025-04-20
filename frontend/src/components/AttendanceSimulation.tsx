import { Form, Input, Modal, notification, Select } from "antd"
import { ChangeEvent, useEffect, useState } from "react"
import { AttendanceQRCodeReqDto, CheckMethod, PasswordAttendanceRequest } from "../services/api"
import { SelectProps } from "antd/lib"
import { QRDeCode } from "../Utils/Utils"
import * as api from '../services/http/httpInstance'

interface Props {
    // 类型
    checkMethod: CheckMethod
    // id
    id: number


    show: boolean
    showChange: (show: boolean) => void
    onFinish: () => void
}

export default (props: Props) => {
    const [isModalOpen, setIsModalOpen] = useState(false)
    const handleCancel = () => {
        setIsModalOpen(false)
    }

    const handleOk = () => {
        onFinish()
    }

    // 密码
    const [pw, setPW] = useState('')
    // 二维码解码字符串
    const [qrCode, setQRCode] = useState('')

    // 考勤类型选项
    const [optionsType, _] = useState<SelectProps['options']>([
        {
            label: '普通',
            value: CheckMethod.Normal
        },
        {
            label: '密码',
            value: CheckMethod.Password
        },
        {
            label: '二维码',
            value: CheckMethod.TowCode
        },
    ])

    useEffect(() => {
        setIsModalOpen(props.show)
    }, [props.show])
    useEffect(() => {
        props.showChange(isModalOpen)
    }, [isModalOpen])

    const onFinish = async () => {

        try {

            if (props.checkMethod == CheckMethod.Normal) {
                await api.Attendance.checkingInPost(props.id)
            }
            else if (props.checkMethod == CheckMethod.Password) {
                await api.Attendance.checkingInPwPost(props.id, { password: pw } as PasswordAttendanceRequest)
            }

            else if (props.checkMethod == CheckMethod.TowCode) {
                await api.Attendance.checkingInQrPost(props.id, { code: qrCode } as AttendanceQRCodeReqDto)
            }

            setIsModalOpen(false)
            props.onFinish()
        } catch (error) {
        }

    }

    const qrFileChange: (event: ChangeEvent<HTMLInputElement>) => void = async (event) => {
        if (!event.target.files) return
        const file = event.target.files[0]
        const res = await QRDeCode(file)
        setQRCode(res)
        if (!res)
            notification.info({ message: '无效二维码' })
        else
            notification.info({ message: '解码数据 ' + res })
    }

    return (
        <Modal
            title='考勤模拟'
            open={isModalOpen}
            onOk={handleOk}
            onCancel={handleCancel}
        >
            <Form layout="vertical">
                <Form.Item<FormData> label='考勤类型'>
                    <Select
                        disabled
                        options={optionsType}
                        value={props.checkMethod}
                    />
                </Form.Item>

                {props.checkMethod == CheckMethod.Password &&
                    <Form.Item label='密码'>
                        <Input value={pw} onChange={e => setPW(e.target.value)} />
                    </Form.Item>
                }
                {props.checkMethod == CheckMethod.TowCode &&
                    <Form.Item label='二维码'>
                        <Input type="file" onChange={qrFileChange} />
                    </Form.Item>
                }
            </Form>
        </Modal>
    )
}