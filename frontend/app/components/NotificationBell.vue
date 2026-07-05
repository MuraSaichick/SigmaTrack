<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useNotificationsStore } from '~/stores/notifications'
import { NotificationType } from '~/types/notification'
import { storeToRefs } from 'pinia'

const notificationsStore = useNotificationsStore()
const { notifications, unreadCount } = storeToRefs(notificationsStore)
const { fetchNotifications, acceptInvitation, rejectInvitation, markAllAsRead, markAsRead } = notificationsStore

const isLoading = ref(false)
const processingId = ref<string | null>(null)

const extractInvitationId = (url?: string): string | null => {
  if (!url) return null
  const match = url.match(/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i)
  return match ? match[0] : null
}

const handleAccept = async (invitationId: string, notificationId: number) => {
  if (isLoading.value) return
  isLoading.value = true
  processingId.value = invitationId
  await acceptInvitation(invitationId, notificationId)
  processingId.value = null
  isLoading.value = false
}

const handleReject = async (invitationId: string, notificationId: number) => {
  if (isLoading.value) return
  isLoading.value = true
  processingId.value = invitationId
  await rejectInvitation(invitationId, notificationId)
  processingId.value = null
  isLoading.value = false
}
interface NotificationVisual {
  icon: string
  colorClass: string
}

const getNotificationVisual = (type: NotificationType): NotificationVisual => {
  const visuals: Record<number, NotificationVisual> = {
    [NotificationType.IssueAssigned]: { icon: 'i-lucide-clipboard-list', colorClass: 'text-purple-500' },
    [NotificationType.IssueStatusChanged]: { icon: 'i-lucide-refresh-cw', colorClass: 'text-amber-500' },
    [NotificationType.NewComment]: { icon: 'i-lucide-message-square', colorClass: 'text-sky-500' },
    [NotificationType.UserMentioned]: { icon: 'i-lucide-at-sign', colorClass: 'text-indigo-500' },

    [NotificationType.ProjectInvitationReceived]: { icon: 'i-lucide-mail-plus', colorClass: 'text-blue-500' },
    [NotificationType.ProjectInvitationAccepted]: { icon: 'i-lucide-user-check', colorClass: 'text-green-500' },
    [NotificationType.ProjectInvitationRejected]: { icon: 'i-lucide-user-x', colorClass: 'text-red-500' },
    [NotificationType.ProjectMemberRemoved]: { icon: 'i-lucide-user-minus', colorClass: 'text-rose-500' },
    [NotificationType.ProjectRoleChanged]: { icon: 'i-lucide-shield-alert', colorClass: 'text-teal-500' },

    [NotificationType.SprintStarted]: { icon: 'i-lucide-play', colorClass: 'text-emerald-500' },
    [NotificationType.SprintCompleted]: { icon: 'i-lucide-check-circle-2', colorClass: 'text-green-600' },

    [NotificationType.SystemAlert]: { icon: 'i-lucide-alert-triangle', colorClass: 'text-red-600 font-bold' },
    [NotificationType.SystemMaintenance]: { icon: 'i-lucide-wrench', colorClass: 'text-orange-500' }
  }

  return visuals[type] || { icon: 'i-lucide-bell', colorClass: 'text-neutral-400 dark:text-neutral-500' }
}

onMounted(() => {
  fetchNotifications()
})
</script>

<template>
  <UPopover mode="click" :popper="{ placement: 'bottom-end', sideOffset: 8 }">
    <UButton variant="ghost" color="neutral" icon="i-lucide-bell" class="relative rounded-xl">
      <div v-if="unreadCount > 0"
        class="absolute top-1.5 right-1.5 w-2 h-2 bg-error-500 rounded-full ring-2 ring-white dark:ring-neutral-950 animate-pulse" />
    </UButton>

    <template #content>
      <div class="w-85 p-2 max-h-[450px] overflow-y-auto flex flex-col justify-start">

        <div
          class="p-2 flex justify-between items-center border-b border-neutral-100 dark:border-neutral-800 shrink-0 mb-1.5">
          <div class="flex items-center gap-2">
            <span class="text-xs font-bold text-neutral-900 dark:text-white">Уведомления</span>
            <span v-if="unreadCount > 0"
              class="text-[10px] bg-error-50 dark:bg-error-950/50 text-error-600 dark:text-error-400 px-2 py-0.5 rounded-full font-black">
              +{{ unreadCount }}
            </span>
          </div>
          <UButton v-if="unreadCount > 0" variant="link" color="neutral" size="xs"
            class="text-[11px] p-0 hover:underline font-medium text-neutral-500 dark:text-neutral-400"
            @click="markAllAsRead">
            Прочитать все
          </UButton>
        </div>

        <div v-if="notifications.length === 0"
          class="py-10 text-center text-xs text-neutral-400 dark:text-neutral-500 flex flex-col items-center gap-2">
          <UIcon name="i-lucide-bell-off" class="w-6 h-6 text-neutral-300 dark:text-neutral-700" />
          Уведомлений нет
        </div>

        <div v-else class="space-y-1 overflow-y-auto flex-1">
          <div v-for="item in notifications" :key="item.id"
            class="p-3 rounded-xl border flex gap-3 relative group transition-all text-left"
            :class="[item.isRead ? 'bg-transparent border-transparent opacity-75' : 'bg-neutral-50 dark:bg-neutral-900/60 border-neutral-100 dark:border-neutral-800/80']">

            <div class="mt-0.5 shrink-0">
              <UIcon :name="getNotificationVisual(item.type).icon"
                :class="[getNotificationVisual(item.type).colorClass, 'w-4 h-4']" />
            </div>

            <div class="flex-1 flex flex-col min-w-0">
              <div class="flex justify-between items-start gap-2">
                <p class="text-xs font-bold text-neutral-900 dark:text-white truncate">
                  {{ item.title }}
                </p>

                <!-- Изменено: Проверка на неравенство типу входящего приглашения -->
                <UButton v-if="!item.isRead && item.type !== NotificationType.ProjectInvitationReceived" size="xs"
                  color="neutral" variant="ghost" icon="i-lucide-check"
                  class="opacity-0 group-hover:opacity-100 transition-opacity p-0.5 h-auto rounded-md"
                  @click="markAsRead(item.id)" />
              </div>

              <p class="text-[11px] text-neutral-500 dark:text-neutral-400 mt-0.5 leading-relaxed wrap-break-words">
                {{ item.message }}
              </p>

              <!-- Изменено: Блок кнопок отображается только для типа входящего приглашения -->
              <div v-if="item.type === NotificationType.ProjectInvitationReceived && extractInvitationId(item.linkUrl)"
                class="flex gap-2 mt-2.5">
                <UButton size="xs" color="success" variant="solid" class="rounded-lg font-bold flex-1 justify-center"
                  :loading="processingId === extractInvitationId(item.linkUrl)"
                  @click="handleAccept(extractInvitationId(item.linkUrl)!, item.id)">
                  Принять
                </UButton>
                <UButton size="xs" color="neutral" variant="soft" class="rounded-lg font-bold flex-1 justify-center"
                  :disabled="processingId === extractInvitationId(item.linkUrl)"
                  @click="handleReject(extractInvitationId(item.linkUrl)!, item.id)">
                  Отклонить
                </UButton>
              </div>

              <div v-else-if="item.linkUrl" class="mt-2 flex justify-start">
                <NuxtLink :to="item.linkUrl" @click="markAsRead(item.id)"
                  class="text-[10px] font-bold text-primary hover:underline flex items-center gap-1">
                  Посмотреть
                  <UIcon name="i-lucide-arrow-right" class="w-2.5 h-2.5" />
                </NuxtLink>
              </div>
            </div>

          </div>
        </div>
      </div>
    </template>
  </UPopover>
</template>