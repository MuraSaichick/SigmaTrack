<script setup lang="ts">
import { onMounted, watch, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useProfileStore } from '~/stores/useProfileStore'
import { useProjectsStore } from '~/stores/useProjectsStore'
import { UserOnlineStatus } from '~/types/user'
import { ProjectRoleEnum } from '~/types/projects'

const route = useRoute()
const profileStore = useProfileStore()
const projectsStore = useProjectsStore()
const config = useRuntimeConfig()
const { t } = useI18n()

const isInviteModalOpen = ref(false)
const selectedProjectIdToInvite = ref<string | undefined>(undefined)

const selectedRoleIdToInvite = ref<ProjectRoleEnum>(ProjectRoleEnum.Developer)
const isInviting = ref(false)

const availableRoles = computed(() => {
  return [
    { id: ProjectRoleEnum.ProjectManager, label: t(`projectRoles.${ProjectRoleEnum.ProjectManager}`) },
    { id: ProjectRoleEnum.Developer, label: t(`projectRoles.${ProjectRoleEnum.Developer}`) },
    { id: ProjectRoleEnum.QAEngineer, label: t(`projectRoles.${ProjectRoleEnum.QAEngineer}`) },
    { id: ProjectRoleEnum.Observer, label: t(`projectRoles.${ProjectRoleEnum.Observer}`) }
  ]
})

const loadProfileData = async () => {
  const userId = route.params.id as string
  if (userId) {
    await profileStore.fetchProfile(userId)
    if (!profileStore.isCurrentUser) {
      await projectsStore.fetchProjects()
    }
  }
}

onMounted(loadProfileData)
watch(() => route.params.id, loadProfileData)

const projectsAvailableForInvitation = computed(() => {
  return projectsStore.projectItems.filter(p => 
    [
      ProjectRoleEnum.ProjectManager, 
      ProjectRoleEnum.Developer, 
      ProjectRoleEnum.QAEngineer
    ].includes(p.currentUserRole as ProjectRoleEnum)
  )
})

const handleSendInvitation = async () => {
  if (!selectedProjectIdToInvite.value || !profileStore.profile?.email) return

  isInviting.value = true
  const success = await projectsStore.inviteUser(selectedProjectIdToInvite.value, {
    inviteeEmail: profileStore.profile.email,
    projectRoleId: selectedRoleIdToInvite.value
  })

  if (success) {
    isInviteModalOpen.value = false
    selectedProjectIdToInvite.value = undefined
    selectedRoleIdToInvite.value = ProjectRoleEnum.Developer
  }
  isInviting.value = false
}

const getAvatarUrl = (url: string | null) => {
  if (!url) return undefined
  if (url.startsWith('http')) return url
  return `${config.public.apiBase}${url}`
}

const formatDate = (dateStr: string | null) => {
  if (!dateStr) return null
  return new Date(dateStr).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

const getOnlineStatusConfig = (status: UserOnlineStatus) => {
  switch (status) {
    case UserOnlineStatus.Online:
      return { 
        label: 'В сети', 
        color: 'bg-emerald-50 text-emerald-600 border border-emerald-200 dark:bg-emerald-500/15 dark:text-emerald-400 dark:border-emerald-500/30', 
        dot: 'bg-emerald-500' 
      }
    case UserOnlineStatus.Away:
      return { 
        label: 'Нет на месте', 
        color: 'bg-amber-50 text-amber-600 border border-amber-200 dark:bg-amber-500/15 dark:text-amber-400 dark:border-amber-500/30', 
        dot: 'bg-amber-500' 
      }
    case UserOnlineStatus.DoNotDisturb:
      return { 
        label: 'Не беспокоить', 
        color: 'bg-rose-50 text-rose-600 border border-rose-200 dark:bg-rose-500/15 dark:text-rose-400 dark:border-rose-500/30', 
        dot: 'bg-rose-500' 
      }
    default:
      return { 
        label: 'Не в сети', 
        color: 'bg-neutral-100 text-neutral-500 border border-neutral-200 dark:bg-neutral-800 dark:text-neutral-400 dark:border-neutral-700/50', 
        dot: 'bg-neutral-400 dark:bg-neutral-600' 
      }
  }
}
</script>

<template>
  <div class="max-w-4xl mx-auto py-8 px-4 sm:px-6 lg:px-8 text-neutral-900 dark:text-neutral-100">
    <div v-if="profileStore.isLoading" class="flex flex-col items-center justify-center py-24">
      <UIcon name="i-lucide-loader-2" class="animate-spin w-12 h-12 text-primary mb-4" />
      <span class="text-neutral-500 dark:text-neutral-400">Загрузка профиля коллеги...</span>
    </div>

    <div v-else-if="profileStore.error" class="text-center py-20">
      <UIcon name="i-lucide-user-x" class="w-16 h-16 mx-auto mb-4 text-rose-500" />
      <h2 class="text-xl font-bold text-neutral-900 dark:text-neutral-100 mb-2">Произошла ошибка</h2>
      <p class="text-neutral-500 dark:text-neutral-400 mb-6">{{ profileStore.error }}</p>
      <UButton color="neutral" variant="subtle" to="/">
        Вернуться на главную
      </UButton>
    </div>

    <div v-else-if="profileStore.profile" class="space-y-6">
      
      <UCard class="bg-white dark:bg-neutral-900/60 border-neutral-200 dark:border-neutral-800 backdrop-blur-md rounded-2xl overflow-hidden relative">
        <div class="flex flex-col md:flex-row items-center md:items-start gap-6 p-4">
          
          <div class="flex flex-col items-center gap-3 shrink-0">
            <div class="relative group">
              <UAvatar
                :src="getAvatarUrl(profileStore.profile.avatarUrl)"
                :alt="`${profileStore.profile.firstname} ${profileStore.profile.lastname}`"
                class="w-32 h-32 rounded-full border-4 border-white dark:border-neutral-800 shadow-xl object-cover font-bold text-3xl text-neutral-900 dark:text-white"
                :style="!profileStore.profile.avatarUrl && profileStore.profile.avatarColor ? `background-color: ${profileStore.profile.avatarColor}` : ''"
              />
              <span 
                class="absolute bottom-1.5 right-1.5 w-4 h-4 rounded-full border-2 border-white dark:border-neutral-900 flex items-center justify-center"
                :class="getOnlineStatusConfig(profileStore.profile.onlineStatus).dot"
              />
            </div>

            <UButton
              v-if="profileStore.isCurrentUser"
              color="primary"
              variant="subtle"
              icon="i-lucide-camera"
              size="xs"
              class="rounded-lg"
            >
              Изменить фото
            </UButton>
          </div>

          <div class="flex-1 text-center md:text-left space-y-3 min-w-0">
            <div>
              <h1 class="text-3xl font-black tracking-tight text-neutral-900 dark:text-neutral-50 truncate">
                {{ profileStore.profile.lastname }} {{ profileStore.profile.firstname }}
                <span v-if="profileStore.profile.patronymic" class="font-medium text-neutral-500 dark:text-neutral-400">
                  {{ profileStore.profile.patronymic }}
                </span>
              </h1>
              <p class="text-sm text-neutral-500 dark:text-neutral-400 mt-1 font-medium">
                {{ profileStore.profile.position || 'Должность не указана' }} 
                <span v-if="profileStore.profile.department" class="text-neutral-300 dark:text-neutral-700 mx-1.5">•</span>
                <span>{{ profileStore.profile.department }}</span>
              </p>
            </div>

            <div class="flex justify-center md:justify-start gap-2">
              <span 
                class="inline-flex items-center gap-1.5 px-3 py-0.5 rounded-full text-xs font-semibold"
                :class="getOnlineStatusConfig(profileStore.profile.onlineStatus).color"
              >
                {{ getOnlineStatusConfig(profileStore.profile.onlineStatus).label }}
              </span>
            </div>

            <div 
              v-if="profileStore.profile.showStatusMessage && profileStore.profile.statusMessage"
              class="inline-flex items-center gap-2 px-3 py-1.5 bg-amber-50 dark:bg-amber-500/10 border border-amber-200 dark:border-amber-500/20 text-amber-600 dark:text-amber-500 text-xs font-semibold rounded-xl"
            >
              <UIcon name="i-lucide-message-square" class="w-4 h-4 shrink-0" />
              <span>{{ profileStore.profile.statusMessage }}</span>
            </div>
          </div>

          <div class="shrink-0 self-center md:self-start">
            <UButton
              v-if="profileStore.isCurrentUser"
              color="neutral"
              variant="outline"
              icon="i-lucide-edit"
              class="rounded-xl border-neutral-200 dark:border-neutral-800 hover:bg-neutral-50 dark:hover:bg-neutral-800 text-neutral-700 dark:text-neutral-300"
              to="/settings/profile"
            >
              Редактировать
            </UButton>
            
            <UButton
              v-else-if="projectsAvailableForInvitation.length > 0 && profileStore.profile.email"
              color="primary"
              icon="i-lucide-user-plus"
              class="rounded-xl"
              @click="void (isInviteModalOpen = true)"
            >
              Пригласить в проект
            </UButton>
          </div>
        </div>
      </UCard>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="md:col-span-2 space-y-6">
          <UCard class="bg-white dark:bg-neutral-900/60 border-neutral-200 dark:border-neutral-800 backdrop-blur-md rounded-2xl">
            <template #header>
              <h3 class="text-sm font-bold text-neutral-800 dark:text-neutral-200 uppercase tracking-wider">Основная информация</h3>
            </template>

            <div class="space-y-3 text-sm">
              <div class="flex items-center gap-4 p-3 rounded-xl bg-neutral-50 dark:bg-neutral-950/20 border border-neutral-100 dark:border-neutral-800/40">
                <UIcon name="i-lucide-user" class="w-5 h-5 text-neutral-500 dark:text-neutral-400 shrink-0" />
                <div class="min-w-0 flex-1">
                  <p class="text-xs text-neutral-400 dark:text-neutral-500">Логин на платформе</p>
                  <p class="font-semibold text-neutral-800 dark:text-neutral-200 break-all">@{{ profileStore.profile.login }}</p>
                </div>
              </div>

              <div class="flex items-center gap-4 p-3 rounded-xl bg-neutral-50 dark:bg-neutral-950/20 border border-neutral-100 dark:border-neutral-800/40">
                <UIcon name="i-lucide-mail" class="w-5 h-5 text-neutral-500 dark:text-neutral-400 shrink-0" />
                <div class="min-w-0 flex-1">
                  <p class="text-xs text-neutral-400 dark:text-neutral-500">Электронная почта</p>
                  <p class="font-semibold text-neutral-800 dark:text-neutral-200 break-all">
                    {{ profileStore.profile.email || 'Скрыто настройками приватности' }}
                  </p>
                </div>
              </div>

              <div class="flex items-center gap-4 p-3 rounded-xl bg-neutral-50 dark:bg-neutral-950/20 border border-neutral-100 dark:border-neutral-800/40">
                <UIcon name="i-lucide-phone" class="w-5 h-5 text-neutral-500 dark:text-neutral-400 shrink-0" />
                <div class="min-w-0 flex-1">
                  <p class="text-xs text-neutral-400 dark:text-neutral-500">Телефон</p>
                  <p class="font-semibold text-neutral-800 dark:text-neutral-200 break-all">
                    {{ profileStore.profile.phoneNumber || 'Не указан или скрыт' }}
                  </p>
                </div>
              </div>

              <div class="flex items-center gap-4 p-3 rounded-xl bg-neutral-50 dark:bg-neutral-950/20 border border-neutral-100 dark:border-neutral-800/40">
                <UIcon name="i-lucide-calendar" class="w-5 h-5 text-neutral-500 dark:text-neutral-400 shrink-0" />
                <div class="min-w-0 flex-1">
                  <p class="text-xs text-neutral-400 dark:text-neutral-500">Дата рождения</p>
                  <p class="font-semibold text-neutral-800 dark:text-neutral-200 break-all">
                    {{ formatDate(profileStore.profile.birthDate) || 'Не указана или скрыта' }}
                  </p>
                </div>
              </div>
            </div>
          </UCard>

          <UCard class="bg-white dark:bg-neutral-900/60 border-neutral-200 dark:border-neutral-800 backdrop-blur-md rounded-2xl">
            <template #header>
              <h3 class="text-sm font-bold text-neutral-800 dark:text-neutral-200 uppercase tracking-wider">О себе</h3>
            </template>
            <p class="text-neutral-600 dark:text-neutral-300 text-sm leading-relaxed whitespace-pre-wrap">
              {{ profileStore.profile.bio || 'Пользователь еще ничего не рассказал о себе.' }}
            </p>
          </UCard>
        </div>

        <div class="space-y-6">
          <UCard class="bg-white dark:bg-neutral-900/60 border-neutral-200 dark:border-neutral-800 backdrop-blur-md rounded-2xl">
            <template #header>
              <h3 class="text-sm font-bold text-neutral-800 dark:text-neutral-200 uppercase tracking-wider">Связь и ресурсы</h3>
            </template>
            <div class="space-y-3">
              <a 
                v-if="profileStore.profile.telegram"
                :href="`https://t.me/${profileStore.profile.telegram.replace('@', '')}`"
                target="_blank"
                class="flex items-center gap-3 p-2.5 rounded-xl bg-white hover:bg-neutral-50 border border-neutral-200 dark:bg-transparent dark:hover:bg-neutral-800/60 dark:border-neutral-800 text-neutral-700 dark:text-neutral-300 transition-all text-sm group"
              >
                <UIcon name="i-lucide-send" class="w-5 h-5 text-sky-500 dark:text-sky-400 shrink-0" />
                <span class="flex-1 truncate font-medium">Telegram</span>
                <span class="text-xs text-neutral-400 dark:text-neutral-500 group-hover:text-primary transition-colors">
                  {{ profileStore.profile.telegram }}
                </span>
              </a>

              <a 
                v-if="profileStore.profile.gitHub"
                :href="`https://github.com/${profileStore.profile.gitHub}`"
                target="_blank"
                class="flex items-center gap-3 p-2.5 rounded-xl bg-white hover:bg-neutral-50 border border-neutral-200 dark:bg-transparent dark:hover:bg-neutral-800/60 dark:border-neutral-800 text-neutral-700 dark:text-neutral-300 transition-all text-sm group"
              >
                <UIcon name="i-lucide-github" class="w-5 h-5 text-neutral-900 dark:text-neutral-50 shrink-0" />
                <span class="flex-1 truncate font-medium">GitHub</span>
                <span class="text-xs text-neutral-400 dark:text-neutral-500 group-hover:text-primary transition-colors">
                  {{ profileStore.profile.gitHub }}
                </span>
              </a>

              <div 
                v-if="!profileStore.profile.telegram && !profileStore.profile.gitHub" 
                class="text-center py-4 text-xs text-neutral-400 dark:text-neutral-500"
              >
                Ссылки отсутствуют
              </div>
            </div>
          </UCard>

          <UCard class="bg-white dark:bg-neutral-900/60 border-neutral-200 dark:border-neutral-800 backdrop-blur-md rounded-2xl">
            <template #header>
              <h3 class="text-sm font-bold text-neutral-800 dark:text-neutral-200 uppercase tracking-wider">Навыки</h3>
            </template>
            <div class="flex flex-wrap gap-1.5">
              <span 
                v-for="skill in profileStore.profile.skills" 
                :key="skill"
                class="px-2.5 py-1 bg-emerald-50 dark:bg-emerald-500/10 border border-emerald-200 dark:border-emerald-500/20 text-emerald-600 dark:text-emerald-500 text-xs font-semibold rounded-lg hover:scale-105 transition-transform cursor-default"
              >
                {{ skill }}
              </span>
              <div 
                v-if="!profileStore.profile.skills || profileStore.profile.skills.length === 0" 
                class="text-center py-4 text-xs text-neutral-400 dark:text-neutral-500 w-full"
              >
                Навыки не указаны
              </div>
            </div>
          </UCard>
        </div>
      </div>
    </div>

    <UModal 
      v-model:open="isInviteModalOpen"
      :ui="{
        content: 'overflow-visible' 
      }"
    >
      <template #content>
        <UCard class="w-full max-w-md bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-2xl p-4 overflow-visible shadow-xl">
          <div class="space-y-4">
            <div class="flex items-center justify-between border-b border-neutral-100 dark:border-neutral-800 pb-3">
              <h3 class="text-lg font-bold text-neutral-900 dark:text-neutral-50">Пригласить в проект</h3>
              <UButton 
                color="neutral" 
                variant="ghost" 
                icon="i-lucide-x" 
                size="sm" 
                @click="void (isInviteModalOpen = false)" 
              />
            </div>

            <p class="text-sm text-neutral-500 dark:text-neutral-400">
              Выберите проект и будущую роль, чтобы отправить приглашение для 
              <span class="text-neutral-900 dark:text-neutral-100 font-semibold">
                {{ profileStore.profile?.firstname }} {{ profileStore.profile?.lastname }}
              </span>.
            </p>

            <UFormField label="Выберите проект" class="text-sm text-neutral-600 dark:text-neutral-400">
              <USelectMenu
                v-model="selectedProjectIdToInvite"
                :items="projectsAvailableForInvitation"
                value-key="id"
                label-key="label"
                placeholder="Выберите проект из списка"
                portal
                class="w-full mt-1 bg-white dark:bg-neutral-950 border border-neutral-200 dark:border-neutral-800 text-neutral-900 dark:text-neutral-100 rounded-xl"
              />
            </UFormField>

            <UFormField label="Роль в проекте" class="text-sm text-neutral-600 dark:text-neutral-400">
              <USelectMenu
                v-model="selectedRoleIdToInvite"
                :items="availableRoles"
                value-key="id"
                label-key="label"
                placeholder="Выберите роль"
                portal
                class="w-full mt-1 bg-white dark:bg-neutral-950 border border-neutral-200 dark:border-neutral-800 text-neutral-900 dark:text-neutral-100 rounded-xl"
              />
            </UFormField>

            <div class="flex justify-end gap-3 pt-4">
              <UButton 
                color="neutral" 
                variant="subtle" 
                @click="void (isInviteModalOpen = false)"
              >
                Отмена
              </UButton>
              <UButton 
                color="primary" 
                :loading="isInviting"
                :disabled="!selectedProjectIdToInvite"
                @click="handleSendInvitation"
              >
                Отправить приглашение
              </UButton>
            </div>
          </div>
        </UCard>
      </template>
    </UModal>
  </div>
</template>