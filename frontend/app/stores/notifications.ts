import { defineStore } from 'pinia'
import type { NotificationDto } from '~/types/notification'
import { useProjectsStore } from '~/stores/projects'

export const useNotificationsStore = defineStore('notifications', () => {
  const api = useApi()
  const toast = useToast()
  const projectsStore = useProjectsStore()
    const router = useRouter() 

  const notifications = ref<NotificationDto[]>([])

  const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

  const fetchNotifications = async () => {
    try {
      notifications.value = await api<NotificationDto[]>('/api/notifications')
    } catch (error) {
      console.error('Ошибка загрузки уведомлений:', error)
    }
  }

  const markAllAsRead = () => {}

  const markAsRead = async (id: number) => {
    try {
      await api(`/api/notifications/${id}/read`, { method: 'POST' })
      const notification = notifications.value.find(n => n.id === id)
      if (notification) notification.isRead = true
    } catch (error) {
      console.error('Ошибка отметки уведомления как прочитанного:', error)
    }
  }

  const acceptInvitation = async (invitationId: string, notificationId: number) => {
    try {
      const response = await api<{ message: string }>(`/api/projects/invitations/${invitationId}/accept`, {
        method: 'POST'
      })

      toast.add({ title: 'Успех', description: response.message, color: 'success' })
      await markAsRead(notificationId)
      await projectsStore.fetchProjects()
      notifications.value = notifications.value.filter(n => n.id !== notificationId)
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось принять приглашение',
        color: 'error'
      })
    }
  }

  const rejectInvitation = async (invitationId: string, notificationId: number) => {
    try {
      const response = await api<{ message: string }>(`/api/projects/invitations/${invitationId}/reject`, {
        method: 'POST'
      })
      toast.add({ title: 'Отклонено', description: response.message, color: 'neutral' })
      await markAsRead(notificationId)
      notifications.value = notifications.value.filter(n => n.id !== notificationId)
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: error.data?.message || 'Не удалось отклонить', color: 'error' })
    }
  }
  const handleNotificationClick = async (notification: NotificationDto) => {
    if (!notification.isRead) {
      await markAsRead(notification.id)
    }
    if (!notification.linkUrl) return
    const projectUuidRegex = /\/projects\/([a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})/i
    const match = notification.linkUrl.match(projectUuidRegex)

    if (match && match[1]) {
      const extractedProjectId = match[1]
      if (projectsStore.selectedProject?.id !== extractedProjectId) {
        await projectsStore.setCurrentProject(extractedProjectId)
      }
    }
    router.push(notification.linkUrl)
  }



  return {
    notifications,
    unreadCount,
    fetchNotifications,
    markAsRead,
    acceptInvitation,
    markAllAsRead,
    rejectInvitation
  }
})