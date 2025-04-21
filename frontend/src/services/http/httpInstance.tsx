import notification from "antd/es/notification";
import axios from "axios";

let baseURL = ''
if (import.meta.env.MODE === 'development') {
     baseURL = 'https://localhost:7019'
} else if (import.meta.env.MODE === 'production') {
     baseURL = ''
}

// 添加请求拦截器
axios.interceptors.request.use(function (config) {
    // 在发送请求之前做些什么
    return config;
}, function (error) {
    // 对请求错误做些什么
    notification.info({
        message: `请求失败，未知错误`,
        placement: "topRight",
    });

    return Promise.reject(error);
});

// 添加响应拦截器
axios.interceptors.response.use(function (response) {
    // 2xx 范围内的状态码都会触发该函数。
    // 对响应数据做点什么
    if (response.data.code != 1) {
        let msg = ''
        switch (response.data.code) {
            case 2:
                msg = '操作失败，未知错误 ' + response.data.msg
                break
            default:
        }
        notification.info({
            message: msg,
            placement: "topRight",
        });
    }
    return response;
}, function (error) {
    // 超出 2xx 范围的状态码都会触发该函数。
    // 对响应错误做点什么
    let msg = ''
    if (error.status == 401) {
        msg = '响应失败，权限不够'
    }
    else {
        msg = '响应失败，未知错误 httpcode ' + error.status
    }
    notification.info({
        message: msg,
        placement: "topRight",
    });
    return Promise.reject(error);
});




import { AccountApi, AttendanceApi, ClassesApi, CourseApi, CourseSelectionApi, TimeTableApi, MajorsCategoryApi, MajorsSubcategoryApi, AttendanceBatchApi } from "../api"
export const Account = new AccountApi(undefined, baseURL, axios)
export const Attendance = new AttendanceApi(undefined, baseURL, axios)
export const Classes = new ClassesApi(undefined, baseURL, axios)
export const Course = new CourseApi(undefined, baseURL, axios)
export const CourseSelection = new CourseSelectionApi(undefined, baseURL, axios)
export const TimeTable = new TimeTableApi(undefined, baseURL, axios)
export const MajorsCategory = new MajorsCategoryApi(undefined, baseURL, axios)
export const MajorsSubcategory = new MajorsSubcategoryApi(undefined, baseURL, axios)
export const AttendanceBatch = new AttendanceBatchApi(undefined, baseURL, axios)
