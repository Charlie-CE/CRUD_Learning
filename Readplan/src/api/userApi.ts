import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7052', // 你的 ASP.NET Core
  headers: {
    'Content-Type': 'application/json',
  },
})

export interface User {
  id: number
  plan_name: string
  planned_hours: number
  actual_hours: number
  abandoned: boolean
}

export interface CreateUserRequest {
  plan_name: string
  planned_hours: number
  actual_hours: number
  abandoned: boolean
}

// 查询全部
export const getAllUser = () => api.get<User[]>('/user')

// 查询单一
export const getUser = (id: number) => api.get<User>(`/user/${id}`)

// 新增
export const createUser = (data: CreateUserRequest) => api.post('/user', data)

// 修改
export const updateUser = (id: number, data: CreateUserRequest) => api.put(`/user/?id=${id}`, data)

// 删除
export const deleteUser = (id: number) => api.delete(`/user/?id=${id}`)
