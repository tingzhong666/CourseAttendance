import axios from "axios";
import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { UserProfile } from "../Models/User";
import * as api from "../services/http/httpInstance"


type UserContextType = {
    user: UserProfile | null
    setUser: React.Dispatch<React.SetStateAction<UserProfile | null>>
    token: string | null
    setToken: React.Dispatch<React.SetStateAction<string | null>>
};
const UserContext = createContext<UserContextType>({} as UserContextType);

interface Props {
    children: ReactNode
}
export const UserProvider = ({ children }: Props) => {
    const [token, setToken] = useState<string | null>(null)
    const [user, setUser] = useState<UserProfile | null>(null)
    const [isReady, setIsReady] = useState(false)

    useEffect(() => {
        const user = localStorage.getItem("user")
        const token = localStorage.getItem("token")


        if (user && token) {
            setUser(JSON.parse(user))
            setToken(token)
            axios.defaults.headers.common["Authorization"] = "Bearer " + token
        }


        // 验证是否有效
        check()

        setIsReady(true)
    }, []);

    const check = async () => {
        var res = await api.Account.apiAccountCheckGet()
        if (res.status == 200) return

        localStorage.setItem("user", "")
        localStorage.setItem("token", "")
        setUser(null)
        setToken(null)
        axios.defaults.headers.common["Authorization"] = ""
    }


    return (
        <UserContext.Provider value={{ token, user, setUser, setToken }}>
            {isReady ? children : null}
        </UserContext.Provider>
    )
}

export const useAuth = () => useContext(UserContext);
