<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useProjectsStore } from '~/stores/projects'
import type { ProjectMemberDto } from '~/types/projects'

const toast = useToast()
const route = useRoute()
const { t } = useI18n()

const projectId = computed(() => route.params.projectId as string)

const projectsStore = useProjectsStore()

const { selectedProject } = storeToRefs(projectsStore)

const { leaveProject, getMembers, fetchProjects } = projectsStore

const { user: currentUser } = useAuth()

const members = ref<ProjectMemberDto[]>([])
const isLoading = ref(true)
const isLeaving = ref(false)
const isConfirmModalOpen = ref(false)

const myMemberInfo = computed<ProjectMemberDto | undefined>(() => {
  if (!members.value) return undefined
  return members.value.find(m => m.email.toLowerCase() === currentUser.value?.email?.toLowerCase())
})

const otherMembers = computed<ProjectMemberDto[]>(() => {
  if (!members.value) return []
  return members.value.filter(m => m.email.toLowerCase() !== currentUser.value?.email?.toLowerCase())
})

const fetchTeam = async () => {
  if (!projectId.value) return
  isLoading.value = true
  const data = await getMembers(projectId.value)
  members.value = data || []
  isLoading.value = false
}

const tryLeaveProject = () => {
  if (myMemberInfo.value?.projectRoleId === 1) {
    toast.add({
      title: t('team.toast.cannotLeave.title'),
      description: t('team.toast.cannotLeave.desc'),
      color: 'warning',
      icon: 'i-lucide-shield-alert'
    })
    return
  }
  isConfirmModalOpen.value = true
}

const handleLeave = async () => {
  if (!projectId.value) return
  isLeaving.value = true
  try {
    const success = await leaveProject(projectId.value)
    if (success) {
      isConfirmModalOpen.value = false
      selectedProject.value = undefined
      await fetchProjects()
      await navigateTo('/')
    }
  } catch (error: any) {
    toast.add({
      title: t('team.toast.error.title'),
      description: error.data?.message || t('team.toast.error.desc'),
      color: 'error'
    })
  } finally {
    isLeaving.value = false
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('ru-RU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

onMounted(() => {
  fetchTeam()
})
</script>

<template>
  <div class="space-y-8 max-w-5xl mx-auto">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4 border-b border-neutral-200 dark:border-neutral-800 pb-5">
      <div>
        <h1 class="text-3xl font-black tracking-tight text-neutral-900 dark:text-white">
          {{ t('team.title') }}
        </h1>
        <p class="text-neutral-500 dark:text-neutral-400 text-sm mt-1">
          {{ t('team.subtitle') }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <UButton 
          color="neutral" 
          variant="soft" 
          icon="i-lucide-refresh-cw" 
          :loading="isLoading" 
          class="rounded-xl font-semibold"
          @click="fetchTeam"
        >
          {{ t('team.refreshBtn') }}
        </UButton>
      </div>
    </div>

    <div v-if="isLoading" class="space-y-4">
      <USkeleton class="h-28 w-full rounded-2xl" />
      <div class="space-y-2">
        <USkeleton class="h-12 w-full rounded-xl" v-for="i in 3" :key="i" />
      </div>
    </div>

    <template v-else>
      <section v-if="myMemberInfo" class="space-y-3">
        <h2 class="text-[11px] font-bold uppercase tracking-wider text-neutral-400 dark:text-neutral-500 px-1">
          {{ t('team.yourStatus') }}
        </h2>
        <div class="bg-white dark:bg-neutral-900 border border-primary-500/30 dark:border-primary-400/20 rounded-2xl p-5 shadow-xs flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-xl bg-primary-500/10 text-primary-600 dark:text-primary-400 font-black text-lg flex items-center justify-center shrink-0 border border-primary-500/20">
              {{ myMemberInfo.firstName.charAt(0).toUpperCase() }}
            </div>
            <div>
              <div class="flex items-center gap-2 flex-wrap">
                <h3 class="font-bold text-neutral-900 dark:text-white text-base">
                  {{ myMemberInfo.firstName }} {{ myMemberInfo.lastname }}
                </h3>
                <UBadge color="primary" variant="subtle" size="sm" class="rounded-md font-bold px-2 py-0.5">
                  {{ t('team.youBadge') }}
                </UBadge>
                <UBadge color="error" variant="soft" size="sm" class="rounded-md font-semibold px-2 py-0.5">
                  {{ myMemberInfo.projectRoleName }}
                </UBadge>
              </div>
              <p class="text-xs text-neutral-400 dark:text-neutral-500 mt-0.5">{{ myMemberInfo.email }}</p>
              <p class="text-[11px] text-neutral-400 dark:text-neutral-500 mt-1.5 flex items-center gap-1">
                <UIcon name="i-lucide-calendar" class="w-3.5 h-3.5" />
                {{ t('team.joinedAt') }} {{ formatDate(myMemberInfo.joinedAt) }}
              </p>
            </div>
          </div>

          <UButton 
            color="error" 
            variant="subtle" 
            icon="i-lucide-log-out" 
            class="font-bold rounded-xl sm:self-center self-start"
            @click="tryLeaveProject"
          >
            {{ t('team.leaveBtn') }}
          </UButton>
        </div>
      </section>

      <section class="space-y-3">
        <h2 class="text-[11px] font-bold uppercase tracking-wider text-neutral-400 dark:text-neutral-500 px-1">
          {{ t('team.teamMembers', { count: otherMembers.length }) }}
        </h2>

        <div v-if="otherMembers.length === 0" class="bg-neutral-50 dark:bg-neutral-900/40 border border-neutral-200 dark:border-neutral-800 rounded-2xl p-10 text-center">
          <UIcon name="i-lucide-users" class="w-8 h-8 text-neutral-300 dark:text-neutral-700 mx-auto mb-2" />
          <p class="text-sm text-neutral-500 dark:text-neutral-400 font-medium">
            {{ t('team.empty.title') }}
          </p>
          <p class="text-xs text-neutral-400 dark:text-neutral-500 mt-1">
            {{ t('team.empty.desc') }}
          </p>
        </div>

        <div v-else class="bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-2xl overflow-hidden shadow-xs">
          <div class="divide-y divide-neutral-100 dark:divide-neutral-800/50">
            <div 
              v-for="member in otherMembers" 
              :key="member.memberId"
              class="p-4 flex items-center justify-between gap-4 hover:bg-neutral-50/60 dark:hover:bg-neutral-800/20 transition-all duration-200"
            >
              <div class="flex items-center gap-3.5 min-w-0">
                <div class="w-10 h-10 rounded-xl bg-neutral-100 dark:bg-neutral-800 text-neutral-700 dark:text-neutral-300 font-bold text-sm flex items-center justify-center shrink-0 border border-neutral-200/40 dark:border-neutral-700/40">
                  {{ member.firstName.charAt(0).toUpperCase() }}
                </div>
                <div class="min-w-0">
                  <p class="text-sm font-bold text-neutral-900 dark:text-white truncate">
                    {{ member.firstName }} {{ member.lastname }}
                    <span v-if="member.patronymic" class="font-normal text-neutral-400 dark:text-neutral-500 text-xs ml-0.5">
                      {{ member.patronymic }}
                    </span>
                  </p>
                  <p class="text-xs text-neutral-400 dark:text-neutral-500 truncate mt-0.5">
                    {{ member.email }}
                  </p>
                </div>
              </div>

              <div class="text-right shrink-0 flex flex-col items-end gap-1">
                <UBadge 
                  :color="member.projectRoleId === 1 ? 'error' : 'neutral'" 
                  variant="subtle" 
                  class="rounded-lg font-bold px-2 py-0.5 text-xs"
                >
                  {{ t(`projectRoles.${member.projectRoleId}`) }}
                </UBadge>
                <span class="text-[10px] text-neutral-400 dark:text-neutral-500">
                  {{ t('team.addedAt') }} {{ formatDate(member.joinedAt) }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </section>
    </template>

    <UModal v-model:open="isConfirmModalOpen">
      <template #content>
        <div class="p-6 space-y-4 bg-white dark:bg-neutral-950 rounded-2xl border border-neutral-200 dark:border-neutral-800 max-w-sm w-full mx-auto shadow-xl">
          <div class="flex gap-3">
            <div class="w-10 h-10 rounded-full bg-error-500/10 text-error-600 dark:text-error-400 flex items-center justify-center shrink-0">
              <UIcon name="i-lucide-alert-triangle" class="w-5 h-5" />
            </div>
            <div class="space-y-1.5">
              <h3 class="text-lg font-bold text-neutral-900 dark:text-white leading-tight">
                {{ t('team.modal.title') }}
              </h3>
              <p class="text-xs text-neutral-500 dark:text-neutral-400 leading-relaxed">
                {{ t('team.modal.desc') }}
              </p>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-2 border-t border-neutral-100 dark:border-neutral-900">
            <UButton 
              color="neutral" 
              variant="ghost" 
              class="rounded-xl font-bold" 
              :disabled="isLeaving" 
              @click="isConfirmModalOpen = false"
            >
              {{ t('team.modal.cancelBtn') }}
            </UButton>
            <UButton 
              color="error" 
              class="rounded-xl font-bold shadow-xs" 
              :loading="isLeaving" 
              @click="handleLeave"
            >
              {{ t('team.modal.confirmBtn') }}
            </UButton>
          </div>
        </div>
      </template>
    </UModal>
  </div>
</template>