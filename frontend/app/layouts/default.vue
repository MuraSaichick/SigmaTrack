<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted, onBeforeUnmount } from 'vue'
import { storeToRefs } from 'pinia'
import { useProjectsStore } from '~/stores/useProjectsStore'
import { useNotificationsStore } from '#imports'
import type { DropdownMenuItem } from '#ui/types'
import type { CreateProjectRequest } from '~/types/projects'

const config = useRuntimeConfig()
const { t, locale } = useI18n()
const route = useRoute()
const { user, logout, token } = useAuth()
const notificationsStore = useNotificationsStore()
const colorMode = useColorMode()

const projectsStore = useProjectsStore()
const { projectItems, selectedProject } = storeToRefs(projectsStore)
const { fetchProjects, createProject } = projectsStore

const toast = useToast()

const newProjectForm = ref<CreateProjectRequest>({
  name: '',
  description: '',
  prefix: ''
})
const isCreating = ref(false)
const isProjectModalOpen = ref(false)

const handleCreateProject = async () => {
  if (isCreating.value) return
  isCreating.value = true
  try {
    const result = await createProject({
      name: newProjectForm.value.name,
      prefix: newProjectForm.value.prefix,
      description: newProjectForm.value.description || undefined
    })
    toast.add({
      title: 'Успех',
      description: `Проект "${result.name}" успешно создан!`,
      color: 'primary'
    })

    newProjectForm.value = { name: '', description: '', prefix: '' }
    isProjectModalOpen.value = false
  } catch (error: any) {
    toast.add({
      title: 'Ошибка создания',
      description: error.data?.message || 'Не удалось создать проект. Проверьте уникальность префикса.',
      color: 'error'
    })
  } finally {
    isCreating.value = false
  }
}

const isDark = computed({
  get() { return colorMode.value === 'dark' },
  set() { colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark' }
})

const toggleLanguage = () => {
  locale.value = locale.value === 'ru' ? 'en' : 'ru'
}

const isMobileMenuOpen = ref(false)

const globalMenuItems = computed(() => [
  { label: t('menu.dashboard'), icon: 'i-lucide-layout-dashboard', to: '/' },
  { label: t('menu.my_tasks'), icon: 'i-lucide-square-check', to: '/tasks' }
])

watch(selectedProject, async (newProject) => {
  if (!newProject) return
  if (route.params.projectId === newProject.id) return
  if (!route.path.startsWith('/projects/')) {
    return; 
  }

  if (route.params.projectId) {
    const pathParts = route.path.split('/')
    const currentTab = pathParts[3]

    if (currentTab === 'issues' && route.params.issueId) {
      await navigateTo(`/projects/${newProject.id}/issues`)
    } else if (currentTab) {
      const newPath = route.path.replace(route.params.projectId as string, newProject.id)
      await navigateTo(newPath)
    } else {
      await navigateTo(`/projects/${newProject.id}/dashboard`)
    }
  } else {
    await navigateTo(`/projects/${newProject.id}/dashboard`)
  }
})

watch(
  () => route.params.projectId,
  async (newProjectId) => {
    if (!newProjectId) return
    if (selectedProject.value?.id !== newProjectId) {
      const currentProject = projectItems.value.find(p => p.id === newProjectId)
      if (currentProject) {
        selectedProject.value = currentProject
      } else {
        await fetchProjects()
      }
    }
  }
)

const projectMenuItems = computed(() => {
  if (!selectedProject.value) return []

  const projectId = selectedProject.value.id
  const items = [
    { label: t('menu.dashboard'), icon: 'i-lucide-layout-dashboard', to: `/projects/${projectId}/dashboard` },
    { label: t('menu.issues'), icon: 'i-lucide-list-todo', to: `/projects/${projectId}/issues` },
    { label: t('menu.kanban'), icon: 'i-lucide-kanban-square', to: `/projects/${projectId}/kanban` },
    { label: t('menu.sprints'), icon: 'i-lucide-clock', to: `/projects/${projectId}/sprints` },
    { label: t('menu.team'), icon: 'i-lucide-users', to: `/projects/${projectId}/team` }
  ]
  if (selectedProject.value.currentUserRole === 1) {
    items.push({
      label: t('menu.project_settings'),
      icon: 'i-lucide-settings-2',
      to: `/projects/${projectId}/settings`
    })
  }
  return items
})

const profileDropdownItems = computed<DropdownMenuItem[]>(() => [
  {
    label: t('menu.my_profile'),
    icon: 'i-lucide-user',
    onSelect: () => { navigateTo('/profile') }
  },
  {
    label: t('menu.account_settings'),
    icon: 'i-lucide-settings',
    onSelect: () => { navigateTo('/settings') }
  },
  { type: 'separator' },
  {
    label: `Язык: ${locale.value.toUpperCase()}`,
    icon: 'i-lucide-languages',
    onSelect: () => { toggleLanguage() }
  },
  {
    label: isDark.value ? 'Светлая тема' : 'Тёмная тема',
    icon: isDark.value ? 'i-lucide-sun' : 'i-lucide-moon',
    onSelect: () => { isDark.value = !isDark.value }
  },
  { type: 'separator' },
  {
    label: t('menu.logout'),
    icon: 'i-lucide-log-out',
    class: 'text-red-600 dark:text-red-400',
    onSelect: () => { logout() }
  }
])

onMounted(async () => {
  fetchProjects()

  if(import.meta.client && token.value) {
    await notificationsStore.fetchNotifications()
    await notificationsStore.startSignalR(token.value)
  }
})  

onBeforeUnmount(async () => {
  if(import.meta.client) {
    await notificationsStore.stopSignalR()
  }
})
</script>

<template>
  <div
    class="min-h-screen bg-neutral-50 dark:bg-neutral-950 text-neutral-900 dark:text-neutral-50 flex transition-colors duration-300">
    
    <header
      class="lg:hidden fixed top-0 left-0 right-0 h-16 bg-white/80 dark:bg-neutral-900/80 backdrop-blur-md border-b border-neutral-200 dark:border-neutral-800 px-4 flex items-center justify-between z-40">
      <h1 class="text-xl font-black tracking-tight text-primary mr-2">
        SIGMA<span class="text-neutral-900 dark:text-white font-semibold">TRACK</span>
      </h1>
      
      <div class="flex items-center gap-3">
        <GlobalSearch />
        
        <NotificationBell />

        <UButton :icon="isMobileMenuOpen ? 'i-lucide-x' : 'i-lucide-menu'" color="neutral" variant="ghost"
          @click="void (isMobileMenuOpen = !isMobileMenuOpen)" />
      </div>
    </header>

    <aside
      :class="[isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0', 'fixed lg:sticky top-0 left-0 bottom-0 w-64 h-screen bg-white dark:bg-neutral-900 border-r border-neutral-200 dark:border-neutral-800 flex flex-col p-4 z-50 transition-transform duration-300 ease-in-out lg:z-0']">
      <div class="space-y-4 shrink-0 pb-2">
        <div class="hidden lg:block text-center pt-2">
          <h1 class="text-2xl font-black tracking-tight text-primary">
            SIGMA<span class="text-neutral-900 dark:text-white font-semibold">TRACK</span>
          </h1>
          <p class="text-[10px] text-neutral-400 dark:text-neutral-500 uppercase tracking-widest font-bold">
            {{ $t('auth.tagline') }}
          </p>
        </div>

        <div
          class="flex items-center gap-2 p-1 bg-neutral-50 dark:bg-neutral-800/40 rounded-2xl border border-neutral-100 dark:border-neutral-800/50">
          <UDropdownMenu :items="profileDropdownItems" :content="{ side: 'bottom', align: 'start', sideOffset: 6 }"
            class="flex-1">
            <template #default>
              <button
                class="flex-1 flex items-center gap-2.5 p-1.5 rounded-xl hover:bg-neutral-200/50 dark:hover:bg-neutral-800 text-left transition-colors min-w-0 group">
                <UAvatar
                  :src="user?.avatarUrl ? (user.avatarUrl.startsWith('http') ? user.avatarUrl : `${config.public.apiBase}${user.avatarUrl}`) : undefined"
                  :alt="`${user?.firstname} ${user?.lastname}`" size="sm"
                  class="shrink-0 font-bold text-xs text-white rounded-lg" />
                <div class="overflow-hidden flex-1">
                  <p
                    class="text-xs font-bold text-neutral-900 dark:text-white truncate group-hover:text-primary transition-colors">
                    {{ user?.firstname }} {{ user?.lastname }}
                  </p>
                  <p class="text-[10px] text-neutral-400 dark:text-neutral-500 truncate">
                    {{ selectedProject ? $t(`projectRoles.${selectedProject.currentUserRole}`) : $t('globalRoles.user')
                    }}
                  </p>
                </div>
                <UIcon name="i-lucide-chevron-down"
                  class="w-3 h-3 text-neutral-400 shrink-0 mr-1 group-hover:text-neutral-600 dark:group-hover:text-neutral-300 transition-colors" />
              </button>
            </template>
          </UDropdownMenu>
          <NotificationBell />
        </div>

        <div class="hidden lg:block px-1">
          <GlobalSearch class="w-full" />
        </div>
      </div>

      <div class="flex-1 overflow-y-auto pr-1 space-y-4">
        <nav class="space-y-1 mt-2">
          <NuxtLink v-for="item in globalMenuItems" :key="item.to" :to="item.to"
            class="flex items-center gap-3 px-3 py-2.5 rounded-xl font-medium text-sm text-neutral-600 dark:text-neutral-400 hover:bg-neutral-100 dark:hover:bg-neutral-800/60 hover:text-neutral-900 dark:hover:text-white transition-all group"
            active-class="!bg-primary/10 !text-primary font-bold">
            <UIcon :name="item.icon"
              class="w-5 h-5 text-neutral-400 group-hover:text-neutral-600 group-[.router-link-active]:text-primary" />
            {{ item.label }}
          </NuxtLink>
        </nav>

        <hr class="border-neutral-200 dark:border-neutral-800" />

        <div class="space-y-1.5">
          <label
            class="text-[11px] font-bold uppercase tracking-wider text-neutral-400 dark:text-neutral-500 px-1 block">
            {{ $t('projects.select') }}
          </label>

          <USelectMenu v-model="selectedProject" :items="projectItems" class="w-full" size="md" />

          <UModal v-model:open="isProjectModalOpen">
            <template #default>
              <UButton color="neutral" variant="ghost" size="xs" icon="i-lucide-plus"
                class="w-full justify-start text-neutral-500 hover:text-neutral-900 dark:hover:text-white mt-1 px-1 rounded-lg">
                Создать новый проект
              </UButton>
            </template>

            <template #content>
              <div
                class="p-6 space-y-4 w-full max-w-md bg-white dark:bg-neutral-950 rounded-2xl border border-neutral-200 dark:border-neutral-800">
                <div>
                  <h3 class="text-lg font-bold text-neutral-900 dark:text-white">Создание нового проекта</h3>
                  <p class="text-xs text-neutral-400 dark:text-neutral-500">Заполните данные для инициализации рабочего
                    пространства</p>
                </div>

                <form @submit.prevent="handleCreateProject" class="space-y-4">
                  <UFormField label="Название проекта">
                    <UInput v-model="newProjectForm.name" placeholder="Например, SigmaTrack Frontend" class="w-full"
                      required :disabled="isCreating" />
                  </UFormField>

                  <UFormField label="Префикс задач" hint="Короткий код (2-5 символов)">
                    <UInput v-model="newProjectForm.prefix" placeholder="Например, SIG" class="w-full" maxlength="5"
                      required :disabled="isCreating"
                      @input="newProjectForm.prefix = newProjectForm.prefix.toUpperCase()" />
                  </UFormField>

                  <UFormField label="Описание (необязательно)">
                    <UTextarea v-model="newProjectForm.description" placeholder="Краткое описание целей проекта..."
                      class="w-full" :disabled="isCreating" :rows="3" />
                  </UFormField>

                  <div class="flex justify-end gap-2 pt-2 border-t border-neutral-100 dark:border-neutral-900">
                    <UButton type="submit" color="primary" class="font-bold rounded-xl" :loading="isCreating">
                      Создать проект
                    </UButton>
                  </div>
                </form>
              </div>
            </template>
          </UModal>
        </div>
        <nav v-if="selectedProject" class="pt-2 space-y-1">
          <label
            class="text-[10px] font-bold uppercase tracking-wider text-neutral-400 dark:text-neutral-500 px-3 block mb-2 truncate">
            Меню проекта: {{ selectedProject.name }}
          </label>
          <NuxtLink v-for="item in projectMenuItems" :key="item.to" :to="item.to"
            class="flex items-center gap-3 px-3 py-2.5 rounded-xl font-medium text-sm text-neutral-600 dark:text-neutral-400 hover:bg-neutral-100 dark:hover:bg-neutral-800/60 hover:text-neutral-900 dark:hover:text-white transition-all group"
            active-class="!bg-primary/10 !text-primary font-bold">
            <UIcon :name="item.icon"
              class="w-5 h-5 text-neutral-400 group-hover:text-neutral-600 group-[.router-link-active]:text-primary" />
            {{ item.label }}
          </NuxtLink>
        </nav>
      </div>
    </aside>

    <div v-if="isMobileMenuOpen" class="fixed inset-0 bg-neutral-950/40 backdrop-blur-xs z-30 lg:hidden"
      @click="isMobileMenuOpen = false" />

    <main class="flex-1 flex flex-col min-w-0 pt-16 lg:pt-0">
      <div class="p-6 lg:p-8 flex-1 max-w-[1600px] w-full mx-auto">
        <slot />
      </div>
    </main>
  </div>
</template>