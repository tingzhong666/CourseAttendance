import { createBrowserRouter } from "react-router-dom"
import { Navigate } from "react-router"
import { CreateUUID } from "../Utils/Utils"
import App from "../App"
import { lazy } from "react"

const Home  = lazy(() => import( "../pages/home/home"))
const Login = lazy(() => import("../pages/login/login"))
const HomeLayout = lazy(() => import("../pages/homeLayout/homeLayout"))
const NotFount = lazy(() => import("../pages/notfount404/notfount404"))
const Courses = lazy(() => import("../pages/courses/courses"))
const Attendance = lazy(() => import("../pages/attendance/attendance"))
const UserManager = lazy(() => import("../pages/user-manager/user-manager"))
const ModifyPw = lazy(() => import("../pages/modify-pw/modify-pw"))
const ModifyUserinfo = lazy(() => import("../pages/modify-userinfo/modify-userinfo"))
const ClassesManager = lazy(() => import("../pages/classes-manager/classes-manager"))
const MajorsSubcategoryManager = lazy(() => import("../pages/majors-subcategory-manager/majors-subcategory-manager"))
const MajorsCategoryManager = lazy(() => import("../pages/majors-category-manager/majors-category-manager"))
const AttendanceBatch = lazy(() => import("../pages/attendanceBatch/attendanceBatch"))

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            {
                path: "", element: <HomeLayout />, children: [
                    { path: "", element: <Navigate to="/home"></Navigate> },
                    { path: "home", element: <Home /> },
                    { path: "courses", element: <Courses key={CreateUUID()}/> },
                    // { path: "my-courses", element: <MyCourses /> },
                    { path: "my-courses", element: <Courses key={CreateUUID()} /> },
                    { path: "attendance", element: <Attendance /> },
                    { path: "attendance-batch", element: <AttendanceBatch /> },
                    { path: "user-manager", element: <UserManager  key={CreateUUID()}/> },
                    { path: "student-manager", element: <UserManager  key={CreateUUID()}/> },
                    // { path: "student-manager", element: <StudentManager /> },
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
])
