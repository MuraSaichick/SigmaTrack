import type { UpdateProfileRequest, UserProfileResponse } from '~/types/user'

export const useProfile = () => {
  const apiFetch = useApi()

  const getProfile = () => 
    apiFetch<UserProfileResponse>('/api/profile', { method: 'GET' })
  
const updateProfile = (body: UpdateProfileRequest) => 
    apiFetch<UserProfileResponse>('/api/profile', {
      method: 'PUT',
      body,
    })

  return {
    getProfile,
    updateProfile
  }
}
