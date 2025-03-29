import { useEffect, useState } from 'react'
import './App.css'
import { Button } from 'antd';
import Login from './pages/login/login';
import HomeLayout from './pages/homeLayout/homeLayout';
import { UserProvider, useAuth } from "./Contexts/auth"
import { Outlet } from 'react-router';
import { useNavigate } from 'react-router';

//import * as api from "./services/http/httpInstance"

const App = () => {
    //let auth = useAuth()
    //let navigate = useNavigate()
    ////await api.Account.apiAccountCheckGet();
    //useEffect(() => {
    //    if (auth.token == null)
    //        navigate("/login")
    //    console.log(auth.token+" 123")
    //}, [])

    return (
        <div className="App">

                {/*{useAuth().token == null ? < Login /> : <HomeLayout />}*/}
                <Outlet />

        </div>
    )
}

export default App
