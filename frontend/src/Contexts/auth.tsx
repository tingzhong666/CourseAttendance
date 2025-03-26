import axios from "axios";
import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { UserProfile } from "../Models/User";
import * as api from "../services/http/httpInstance"
import { useNavigate } from "react-router";


type UserContextType = {
    user: UserProfile | null
    setUser: React.Dispatch<React.SetStateAction<UserProfile | null>>
    token: string | null
    setToken: React.Dispatch<React.SetStateAction<string | null>>
    logout: () => void
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
        const user = localStorage.getItem("user")
        const token = localStorage.getItem("token")


        if (user && token) {
            setUser(JSON.parse(user))
            setToken(token)
            axios.defaults.headers.common["Authorization"] = "Bearer " + token
        }


        // ��֤�Ƿ���Ч
        check()

        setIsReady(true)
    }, []);

    const check = async () => {
        var res = await api.Account.apiAccountCheckGet()
        if (res.status == 200) return
        logout()
    }

    const logout = () => {
        localStorage.removeItem("user")
        localStorage.removeItem("token")
        setUser(null)
        setToken(null)
        axios.defaults.headers.common["Authorization"] = ""
        navigate("/login")
    }

    return (
        <UserContext.Provider value={{ token, user, setUser, setToken, logout }}>
            {isReady ? children : null}
        </UserContext.Provider>
    )
}

export const useAuth = () => useContext(UserContext);
