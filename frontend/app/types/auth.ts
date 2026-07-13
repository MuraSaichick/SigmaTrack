export enum GlobalRoleEnum {
  Admin = 1,
  User = 2
}

export interface User {
  userId: string
  login: string
  roleId: GlobalRoleEnum
  firstname: string
  patronymic?: string
  lastname: string
  email: string
  phone: string
  createdAt: string
}

export interface LoginRequest {
  login: string
  password: string
}

export interface LoginResponse {
  id: string
  token: string
  login: string
  email: string
  firstname: string
  lastname: string
  avatarUrl: string | null
}

export interface RegisterRequest {
  login: string
  email: string
  phone: string
  password: string
  firstname: string
  lastname: string
  patronymic?: string
}