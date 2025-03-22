import notification from "antd/es/notification";
import axios from "axios";
import React from "react"

const baseURL = 'https://localhost:5246/'

const instance = axios.create({
    baseURL,
    timeout: 1000,
});

const [api, contextHolder] = notification.useNotification();
// �������������
instance.interceptors.request.use(function (config) {
    // �ڷ�������֮ǰ��Щʲô
    return config;
}, function (error) {
    // �����������Щʲô
    api.info({
        message: `����ʧ�ܣ�δ֪����`,
        placement: "topRight",
    });
    return Promise.reject(error);
});

// �����Ӧ������
instance.interceptors.response.use(function (response) {
    // 2xx ��Χ�ڵ�״̬�붼�ᴥ���ú�����
    // ����Ӧ��������ʲô
    return response;
}, function (error) {
    // ���� 2xx ��Χ��״̬�붼�ᴥ���ú�����
    // ����Ӧ��������ʲô
    api.info({
        message: `��Ӧʧ�ܣ�δ֪����`,
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
