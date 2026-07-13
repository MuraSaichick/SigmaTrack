import { defineStore } from 'pinia'
import type { UserSearchResultDto } from '~/types/projects'

export const useUsersStore = defineStore('users', () => {
  const api = useApi()
  const { user: currentUser } = useAuth()

  const searchUsers = async (query: string, limit: number = 10): Promise<UserSearchResultDto[]> => {
    if (!query.trim()) return []
    try {
      const data = await api<UserSearchResultDto[]>(`/api/users/search`, {
        method: 'GET',
        params: { query, limit }
      })
      return data.filter(u => u.id !== currentUser.value?.id && u.email.toLowerCase() !== currentUser.value?.email?.toLowerCase())
    } catch (error) {
      console.error('Ошибка при поиске пользователей:', error)
      return []
    }
  }
  return {
    searchUsers
  }
})