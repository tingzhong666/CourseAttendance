import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import Home from "../pages/home/home";
import Login from "../pages/login/login";
import HomeLayout from "../pages/homeLayout/homeLayout";
import NotFount from "../pages/notfount404/notfount404";
import Courses from "../pages/courses/courses";
import MyCourses from "../pages/my-courses/my-courses";
import Attendance from "../pages/attendance/attendance";
import UserManager from "../pages/user-manager/user-manager";
import StudentManager from "../pages/student-manager/student-manager";
import ModifyPw from "../pages/modify-pw/modify-pw";
import ModifyUserinfo from "../pages/modify-userinfo/modify-userinfo";
import ClassesManager from "../pages/classes-manager/classes-manager";
import MajorsSubcategoryManager from "../pages/majors-subcategory-manager/majors-subcategory-manager";
import MajorsCategoryManager from "../pages/majors-category-manager/majors-category-manager";
import { Navigate } from "react-router";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            {
                path: "", element: <HomeLayout />, children: [
                    { path: "", element: <Navigate to="/home"></Navigate> },
                    { path: "home", element: <Home /> },
                    { path: "courses", element: <Courses /> },
                    { path: "my-courses", element: <MyCourses /> },
                    { path: "attendance", element: <Attendance /> },
                    { path: "user-manager", element: <UserManager /> },
                    { path: "student-manager", element: <StudentManager /> },
                    { path: "modify-pw", element: <ModifyPw /> },
                    { path: "modify-userinfo", element: <ModifyUserinfo /> },
                    { path: "classes-manager", element: <ClassesManager /> },
                    { path: "majors-category-manager", element: <MajorsCategoryManager /> },
                    { path: "majors-subcategory-manager", element: <MajorsSubcategoryManager /> },
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
