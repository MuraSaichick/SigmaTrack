import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { 
  PrivacySettings, 
  UpdateEmailRequest, 
  UpdatePasswordRequest 
} from '~/types/settings'

export const useSettingsStore = defineStore('settings', () => {
  const privacy = ref<PrivacySettings | null>(null)
  const isLoading = ref(false)
  
  const toast = useToast()
  const apiFetch = useApi() 

  async function fetchPrivacy() {
    isLoading.value = true
    try {
      const data = await apiFetch<PrivacySettings>('/api/settings/privacy')
      privacy.value = data
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.detail || 'Не удалось загрузить настройки приватности',
        color: 'error'
      })
    } finally {
      isLoading.value = false
    }
  }

  async function updatePrivacy(newSettings: PrivacySettings) {
    isLoading.value = true
    try {
      await apiFetch('/api/settings/privacy', {
        method: 'PUT',
        body: newSettings
      })
      privacy.value = { ...newSettings }
      toast.add({
        title: 'Успешно сохранено',
        description: 'Параметры приватности обновлены',
        color: 'success',
        icon: 'i-lucide-shield-check'
      })
    } catch (error: any) {
      toast.add({
        title: 'Ошибка сохранения',
        description: error.data?.detail || 'Попробуйте позже',
        color: 'error'
      })
      throw error
    } finally {
      isLoading.value = false
    }
  }

  async function updateEmail(payload: UpdateEmailRequest) {
    isLoading.value = true
    try {
      await apiFetch('/api/settings/change-email', {
        method: 'POST',
        body: payload
      })
      toast.add({
        title: 'Email изменен',
        description: 'Новый адрес электронной почты успешно сохранен',
        color: 'success',
        icon: 'i-lucide-mail'
      })
    } catch (error: any) {
      toast.add({
        title: 'Ошибка изменения email',
        description: error.data?.detail || 'Возможно, данный email уже занят',
        color: 'error'
      })
      throw error
    } finally {
      isLoading.value = false
    }
  }
  async function updatePassword(payload: UpdatePasswordRequest) {
    isLoading.value = true
    try {
      await apiFetch('/api/settings/change-password', {
        method: 'POST',
        body: payload
      })
      toast.add({
        title: 'Пароль изменен',
        description: 'Ваш пароль безопасности успешно обновлен',
        color: 'success',
        icon: 'i-lucide-key-round'
      })
    } catch (error: any) {
      toast.add({
        title: 'Ошибка смены пароля',
        description: error.data?.detail || 'Убедитесь в правильности текущего пароля',
        color: 'error'
      })
      throw error
    } finally {
      isLoading.value = false
    }
  }

  return {
    privacy,
    isLoading,
    fetchPrivacy,
    updatePrivacy,
    updateEmail,
    updatePassword
  }
})