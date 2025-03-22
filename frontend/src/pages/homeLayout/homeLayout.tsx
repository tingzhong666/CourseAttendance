import React from 'react'
import "./homeLayout.css"
import { Col, Row } from 'antd'
import SideMenu from './SideMenu/SideMenu'
import Home from '../home/home'

interface Props {

}

export default (props: Props) => {
    return (
        <>
            <Row>
                <Col span={5} style={{ border: '1px solid ' }}>
                    <SideMenu/>
                </Col>
                <Col span={19} style={{ border: '1px solid ' }}>
                    <Home/>
                </Col>
            </Row>
        </>
    )
}