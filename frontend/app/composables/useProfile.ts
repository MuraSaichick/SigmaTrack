import type { UpdateProfileRequest, UserProfileResponse } from '~/types/user'

export const useProfile = () => {
  const { token } = useAuth()
  const config = useRuntimeConfig()

  const fetchWithAuth = <T>(url: string, options: any = {}) => {
    return $fetch<T>(url, {
      baseURL: config.public.apiBase || 'http://localhost:5000',
      ...options,
      headers: {
        Authorization: `Bearer ${token.value}`,
        ...options.headers,
      },
    })
  }

  const getProfile = () => fetchWithAuth<UserProfileResponse>('/api/profile', { method: 'GET' })
  
  const updateProfile = (body: UpdateProfileRequest) => 
    fetchWithAuth<UserProfileResponse>('/api/profile', {
      method: 'PUT',
      body,
    })

  return {
    getProfile,
    updateProfile
  }
}
