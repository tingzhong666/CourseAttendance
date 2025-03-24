import axios from "axios";
import { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { UserProfile } from "../Models/User";


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
    const [token, setToken] = useState<string | null>(null);
    const [user, setUser] = useState<UserProfile | null>(null);
    const [isReady, setIsReady] = useState(false);

    useEffect(() => {
        const user = localStorage.getItem("user");
        const token = localStorage.getItem("token");
        if (user && token) {
            setUser(JSON.parse(user));
            setToken(token);
            axios.defaults.headers.common["Authorization"] = "Bearer " + token;
        }
        setIsReady(true);
    }, []);


    return (
        <UserContext.Provider value={{ token, user, setUser, setToken }}>
            {isReady ? children : null}
        </UserContext.Provider>
    )
}

export const useAuth = () => useContext(UserContext);
