import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import Home from "../pages/home/home";
import Login from "../pages/login/login";
import HomeLayout from "../pages/homeLayout/homeLayout";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            {
                path: "", element: <HomeLayout />, children: [
                    { path: "", element: <Home /> }
                ]
            },
            { path: "login", element: <Login /> },
            //{ path: "company/:ticker", element: <CompanyPage /> },
        ],
    },
]);