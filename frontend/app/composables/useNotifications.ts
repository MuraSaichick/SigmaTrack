type NotificationType = 'ProjectInvitation' | 'TaskAssigned' | 'SystemAlert'

export interface NotificationDto {
  id: number
  type: NotificationType
  title: string
  message: string
  isRead: boolean
  linkUrl?: string
  createdAt: string
}

const notifications = ref<NotificationDto[]>([])

export const useNotifications = () => {
  const api = useApi()

  const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

  const fetchNotifications = async () => {
    try {
      notifications.value = await api<NotificationDto[]>('/api/notifications')
    } catch (error) {
      console.error('Ошибка загрузки уведомлений:', error)
    }
  }

  const markAsRead = async (id: number) => {
    try {
      await api(`/api/notifications/${id}/read`, {
        method: 'POST'
      })
      
      const notification = notifications.value.find(n => n.id === id)
      if (notification) {
        notification.isRead = true
      }
    } catch (error) {
      console.error('Ошибка отметки уведомления:', error)
    }
  }

  return {
    notifications,
    unreadCount,
    fetchNotifications,
    markAsRead
  }
}