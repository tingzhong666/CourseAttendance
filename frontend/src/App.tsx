import { useState } from 'react'
import './App.css'
import { Button } from 'antd';
import Login from './pages/login/login';
import HomeLayout from './pages/homeLayout/homeLayout';

//import * as api from "./services/http/httpInstance"

const App = () => {

    //await api.Account.apiAccountCheckGet();

    return (
        <div className="App">
            {/*<Login/>*/}
            <HomeLayout />
        </div>
    )
}

export default App
