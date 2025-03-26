import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import Home from "../pages/home/home";
import Login from "../pages/login/login";
import HomeLayout from "../pages/homeLayout/homeLayout";
import NotFount from "../pages/notfount404/notfount404";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            {
                path: "", element: <HomeLayout />, children: [
                    { path: "home", element: <Home /> }
                ]
            },
            { path: "login", element: <Login /> },
            //{ path: "company/:ticker", element: <CompanyPage /> },
        ],
    },
    {
        path: "*",
        element: <NotFount />
    }
]);
