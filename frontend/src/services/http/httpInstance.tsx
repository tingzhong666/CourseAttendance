import notification from "antd/es/notification";
import axios from "axios";
import React from "react"

const baseURL = 'https://localhost:5246/'

// �������������
axios.interceptors.request.use(function (config) {
    // �ڷ�������֮ǰ��Щʲô
    return config;
}, function (error) {
    // �����������Щʲô
    notification.info({
        message: `����ʧ�ܣ�δ֪����`,
        placement: "topRight",
    });
    
    return Promise.reject(error);
});

// �����Ӧ������
axios.interceptors.response.use(function (response) {
    // 2xx ��Χ�ڵ�״̬�붼�ᴥ���ú�����
    // ����Ӧ��������ʲô
    return response;
}, function (error) {
    // ���� 2xx ��Χ��״̬�붼�ᴥ���ú�����
    // ����Ӧ��������ʲô
    notification.info({
        message: `��Ӧʧ�ܣ�δ֪����`,
        placement: "topRight",
    });
    return Promise.reject(error);
});




import { AcademicApi, AccountApi, AdminApi, AttendanceApi, ClassesApi, CourseApi, CourseSelectionApi, StudentApi, TeacherApi } from "../api"
export const Academic = new AcademicApi(undefined, baseURL,axios )
export const Account = new AccountApi(undefined, baseURL, axios)
export const Admin = new AdminApi(undefined, baseURL, axios)
export const Attendance = new AttendanceApi(undefined, baseURL, axios)
export const Classes = new ClassesApi(undefined, baseURL, axios)
export const Course = new CourseApi(undefined, baseURL, axios)
export const CourseSelection = new CourseSelectionApi(undefined, baseURL, axios)
export const Student = new StudentApi(undefined, baseURL, axios)
export const Teacher = new TeacherApi(undefined, baseURL, axios)
