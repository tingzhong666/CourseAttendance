import React from 'react'
import "./login.css"
import { Button, Card, Flex, Form, Input, Image, Row, Col } from "antd"
import loginPng from "../../assets/login.png"
import { LockOutlined, UserOutlined } from '@ant-design/icons';

interface Props {

}

interface formType {
    username: string,
    password: string
}

export default (props: Props) => {


    const [form] = Form.useForm();

    const onFinish = (values: formType) => {
        console.log("asdasdasdasd" + values.username);
    };
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
                                </Form.Item>
                            </Form>
                        </Card>
                    </Col>
                </Row>

            </Card>
        </div>
    )
}