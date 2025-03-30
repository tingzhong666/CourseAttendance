import React from 'react'
import "./login.css"
import { Button, Card, Flex, Form, Input, Image, Row, Col, notification } from "antd"
import loginPng from "../../assets/login.png"
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import * as api from "../../services/http/httpInstance"
import { LoginModel } from '../../services/api';
import { useAuth } from "../../Contexts/auth"
import { UserProfile } from '../../Models/User';
import { useNavigate } from 'react-router';

interface Props {

}

interface formType {
    username: string
    password: string
}

export default (props: Props) => {
    const auth = useAuth();
    const navigate = useNavigate();

    const [form] = Form.useForm();

    const onFinish = async (values: formType) => {
        var res = await api.Account.apiAccountLoginPost({ userName: values.username, password: values.password } as LoginModel);

        if (res.data.code != 1) {
            switch (res.data.code) {
                case 3:
                    notification.info({
                        message: `请求失败，无效的工号或密码`,
                        placement: "topRight",
                    })
                    break
            }
            return;
        }

        var data = res.data.data

        //auth.setToken(data?.token || null)
        //let userProfile = { userName: data?.userName } as UserProfile
        //auth.setUser(userProfile)

        //const user = localStorage.setItem("user", JSON.stringify(userProfile));
        const token = localStorage.setItem("token", data?.token || "");

        navigate("/home")
    }



    const onTest = async (e: React.MouseEvent<HTMLElement, MouseEvent>) => {
        console.log(auth.token + "qwe")
        //var res = await api.Account.apiAccountTestGet();
    }

    return (
        <div className="login">
            {/*标题*/}
            <div className="title">
                四川轻化工大学课程考勤系统
            </div>

            <Card
                style={{
                    width: 1000,
                    margin: "0 auto",
                    boxShadow: '0 1px 2px 0 rgba(0, 0, 0, 0.03), 0 1px 6px -1px rgba(0, 0, 0, 0.32), 0 2px 4px 0 rgba(0, 0, 0, 0.02)',
                    padding: 0,
                    background: "#f5f5f5"
                }} variant="borderless">
                <Row gutter={16}>
                    <Col span={16}>
                        {/*展示图片*/}
                        <Image width="100%" src={loginPng} preview={false} />
                    </Col>

                    <Col span={8}>
                        {/*表单*/}
                        <Card title="用户登录" variant="borderless">
                            <Form
                                layout="vertical"
                                form={form}
                                onFinish={onFinish}
                            >
                                <Form.Item<formType> name="username">
                                    <Input placeholder="学号/工号" prefix={<UserOutlined />} />
                                </Form.Item>
                                <Form.Item<formType> name="password">
                                    <Input.Password placeholder="密码" prefix={<LockOutlined />} />
                                </Form.Item>
                                <Form.Item>
                                    <Button type="primary" htmlType="submit">登录</Button>
                                    <Button type="primary" onClick={e => onTest(e)}>测试</Button>
                                </Form.Item>
                            </Form>
                        </Card>
                    </Col>
                </Row>

            </Card>
        </div>
    )
}