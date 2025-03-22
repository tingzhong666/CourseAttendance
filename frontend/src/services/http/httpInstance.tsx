import notification from "antd/es/notification";
import axios from "axios";
import React from "react"

const baseURL = 'https://localhost:5246/'

const instance = axios.create({
    baseURL,
    timeout: 1000,
});

const [api, contextHolder] = notification.useNotification();
// 添加请求拦截器
instance.interceptors.request.use(function (config) {
    // 在发送请求之前做些什么
    return config;
}, function (error) {
    // 对请求错误做些什么
    api.info({
        message: `请求失败，未知错误`,
        placement: "topRight",
    });
    return Promise.reject(error);
});

// 添加响应拦截器
instance.interceptors.response.use(function (response) {
    // 2xx 范围内的状态码都会触发该函数。
    // 对响应数据做点什么
    return response;
}, function (error) {
    // 超出 2xx 范围的状态码都会触发该函数。
    // 对响应错误做点什么
    api.info({
        message: `响应失败，未知错误`,
        placement: "topRight",
    });
    return Promise.reject(error);
});



import { AcademicApi, AccountApi, AdminApi, AttendanceApi, ClassesApi, CourseApi, CourseSelectionApi, StudentApi, TeacherApi } from "../api"
export const Academic = new AcademicApi(undefined, baseURL, instance)
export const Account = new AccountApi(undefined, baseURL, instance)
export const Admin = new AdminApi(undefined, baseURL, instance)
export const Attendance = new AttendanceApi(undefined, baseURL, instance)
export const Classes = new ClassesApi(undefined, baseURL, instance)
export const Course = new CourseApi(undefined, baseURL, instance)
export const CourseSelection = new CourseSelectionApi(undefined, baseURL, instance)
export const Student = new StudentApi(undefined, baseURL, instance)
export const Teacher = new TeacherApi(undefined, baseURL, instance)
