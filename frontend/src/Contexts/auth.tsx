import axios from "axios";
import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { UserProfile } from "../models/User";
import * as api from "../services/http/httpInstance"
import { useNavigate } from "react-router";
import { notification } from "antd";


type UserContextType = {
    user: UserProfile | null
    //setUser: React.Dispatch<React.SetStateAction<UserProfile | null>>
    token: string | null
    //setToken: React.Dispatch<React.SetStateAction<string | null>>
    logout: () => void,
    roleMap: (role: string) => string
};
const UserContext = createContext<UserContextType>({} as UserContextType);

interface Props {
    children: ReactNode
}
export const UserProvider = ({ children }: Props) => {
    const [token, setToken] = useState<string | null>(null)
    const [user, setUser] = useState<UserProfile | null>(null)
    const [isReady, setIsReady] = useState(false)
    const navigate = useNavigate()

    useEffect(() => {
        init()
    }, []);

    const init = async () => {
        const token = localStorage.getItem("token")
        if (token) {
            setToken(token)
            axios.defaults.headers.common["Authorization"] = "Bearer " + token
        }

        // 验证是否有效
        if (await check())
            // 获取信息
            await getUserInfo()

        setIsReady(true)
    }

    const getUserInfo = async () => {
        var res = await api.Account.apiAccountProfileSelfGet()
        if (res.data.code != 1) {
            let msg = ''
            switch (res.data.code) {
                case 4:
                    msg = '获取当前用户ID失败'
                    break
                case 5:
                    msg = '获取当前用户信息失败'
                    break
            }
            notification.info({
                message: msg,
                placement: "topRight",
            });
            return
        }

        let data = res.data.data

        setUser({
            userName: data?.userName,
            roles: data?.roles,
            name: data?.name,
            id: data?.id
        } as UserProfile)
    }

    const check = async () => {
        let res = await api.Account.apiAccountCheckGet()
        if (res.data.code != 1) {
            navigate('/login')
            return false
        }
        return true
    }

    const logout = () => {
        //localStorage.removeItem("user")
        localStorage.removeItem("token")
        //setUser(null)
        setToken(null)
        axios.defaults.headers.common["Authorization"] = ""
        navigate("/login")
    }

    // 身份映射
    const roleMap = (role: string): string => {
        switch (role) {
            case 'Admin':
                return '管理员'
            case 'Academic':
                return '教务处人员'
            case 'Teacher':
                return '老师'
            case 'Student':
                return '学生'
            default:
                return ''
        }
    }


    return (
        <UserContext.Provider value={{ token, logout, user, roleMap }}>
            {isReady ? children : null}
        </UserContext.Provider>
    )
}

export const useAuth = () => useContext(UserContext);

