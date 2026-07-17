import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { UserProfileResponse } from '~/types/user'
import { useAuth } from '~/composables/useAuth'

export const useProfileStore = defineStore('profile', () => {
  const api = useApi()
  const { user: currentUser } = useAuth()

  const profile = ref<UserProfileResponse | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const isCurrentUser = computed(() => {
    if (!profile.value || !currentUser.value) return false
    return profile.value.id === currentUser.value.id
  })

  async function fetchProfile(userId: string) {
    isLoading.value = true
    error.value = null
    try {
      const data = await api<UserProfileResponse>(`/api/users/${userId}/profile`)
      profile.value = data
    } catch (err: any) {
      error.value = err.data?.message || 'Не удалось загрузить профиль'
      profile.value = null
    } finally {
      isLoading.value = false
    }
  }

  function clearProfile() {
    profile.value = null
    error.value = null
  }

  return {
    profile,
    isLoading,
    error,
    isCurrentUser,
    fetchProfile,
    clearProfile
  }
})