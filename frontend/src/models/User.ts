import { UserRole } from "../services/api";

export type UserProfile = {
    // ����ѧ��
    userName: string;
    //email: string;
    // ����
    roles: [UserRole],
    // ����
    name: string,
    // id
    id: string
};