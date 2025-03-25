﻿import React, { useEffect } from 'react'
import "./homeLayout.css"
import { Col, Row } from 'antd'
import SideMenu from './SideMenu/SideMenu'
import Home from '../home/home'
import { Outlet, useNavigate,  } from 'react-router'
import { useAuth } from '../../Contexts/auth'

interface Props {

}

export default (props: Props) => {


    let auth = useAuth()
    let navigate = useNavigate()
    //await api.Account.apiAccountCheckGet();
    useEffect(() => {
        if (auth.token == null)
            navigate("/login")
        //console.log(auth.token + " 123")
    }, [])


    return (
        <>
            <Row>
                <Col span={5} style={{ border: '1px solid ' }}>
                    <SideMenu />
                </Col>
                <Col span={19} style={{ border: '1px solid ' }}>
                    {/*<Home />*/}
                    <Outlet />
                </Col>
            </Row>
        </>
    )
}