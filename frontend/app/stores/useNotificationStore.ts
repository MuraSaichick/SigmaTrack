import type { NotificationDto } from '~/types/notification'
import { useProjectsStore } from '~/stores/useProjectsStore'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

export const useNotificationsStore = defineStore('notifications', () => {
  const api = useApi()
  const toast = useToast()
  const projectsStore = useProjectsStore()

  const notifications = ref<NotificationDto[]>([])

  const hubConnection = ref<HubConnection | null>(null)
  const isConnected = ref(false)
  const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

  const startSignalR = async (token: string) => {
    if (hubConnection.value) return
    const config = useRuntimeConfig()
    const connection = new HubConnectionBuilder()
      .withUrl(`${config.public.apiBase}/hubs/notifications`,
        {
          accessTokenFactory: () => token
        }
      )
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    connection.on('ReceiveNotification', (newNotification: NotificationDto) => {
      notifications.value.unshift(newNotification)
      toast.add({
        title: newNotification.title,
        description: newNotification.message,
        icon: 'i-heroicons-bell-alert',
        color: 'primary',
        actions: newNotification.linkUrl ? [
          {
            label: 'Открыть',
            onClick: () => handleNotificationClick(newNotification)
          }
        ] : []
      })
    })
    try {
      await connection.start()
      hubConnection.value = connection
      isConnected.value = true
      console.log('SignalR подключен к хабу уведомлений.')
    } catch (err) {
      console.error('Ошибка при старте SignalR:', err)
    }
  }
  const stopSignalR = async () => {
      if (hubConnection.value) {
        await hubConnection.value.stop()
      hubConnection.value = null
      isConnected.value = false
      console.log('SignalR отключен.')
    }
    }

  const fetchNotifications = async () => {
    try {
      notifications.value = await api<NotificationDto[]>('/api/notifications')
    } catch (error) {
      console.error('Ошибка загрузки уведомлений:', error)
    }
  }

  const markAllAsRead = async () => {
    if (unreadCount.value === 0) return
    try {
      await api('/api/notifications/read-all', { method: 'POST' })
      notifications.value.forEach(n => n.isRead = true)
    } catch (error) {
      console.error('Ошибка отметки всех уведомлений:', error)
    }
  }

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
      notifications.value = notifications.value.filter(n => n.id !== notificationId)
      await projectsStore.fetchProjects()
      markAsRead(notificationId);
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
      notifications.value = notifications.value.filter(n => n.id !== notificationId)
      markAsRead(notificationId);
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
    await navigateTo(notification.linkUrl)
  }

  const clearStore = () => {
    notifications.value = []
    stopSignalR()
  }

  return {
    notifications,
    unreadCount,
    fetchNotifications,
    startSignalR,
    stopSignalR,
    markAsRead,
    markAllAsRead,
    acceptInvitation,
    rejectInvitation,
    handleNotificationClick,
    clearStore
  }
})
