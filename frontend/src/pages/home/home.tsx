import React from 'react';
import { useAuth } from '../../Contexts/auth';
import userLogo from '../../assets/user_logo.jpg'
import { Avatar, Col, Row } from "antd"

const Home = () => {
    const auth = useAuth()

    const textCenter: React.CSSProperties = {
        display: 'flex',
        height: '100%',
        alignItems: 'center',
    }
    const title: React.CSSProperties = {
        justifyContent: 'center',
        width: '150px'
    }

    return (
        <>
            <Row>
                <Col style={{ ...textCenter, ...title }}>
                    <Avatar src={<img src={userLogo} alt="avatar" />} size={100} />
                </Col>
                <Col>
                    <div style={textCenter}>
                        {auth.user?.name}
                    </div>
                </Col>
            </Row>

            <Row style={{ height: '50px' }}>
                <Col style={{ ...textCenter, width: '200px', ...title }}>
                    身份
                </Col>
                <Col style={textCenter}>
                    {auth.user?.roles.map(auth.roleMap)}
                </Col>
            </Row>

            <Row style={{ height: '50px' }}>
                <Col style={{ ...textCenter, width: '200px', ...title }}>
                    {auth.user?.roles.includes('Student') ? '学号' : '工号'}
                </Col>
                <Col style={textCenter}>
                    {auth.user?.userName}
                </Col>
            </Row>
        </>
    );
}

export default Home