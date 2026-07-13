<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useProjectsStore } from '~/stores/useProjectsStore'
import { useUsersStore } from '~/stores/useUsersStore'
import type { UpdateProjectDetailsRequest, UserSearchResultDto } from '~/types/projects'

const { t } = useI18n()
const route = useRoute()
const toast = useToast()

const projectsStore = useProjectsStore()
const usersStore = useUsersStore()

const { selectedProject } = storeToRefs(projectsStore)

const {
    getMembers,
    updateProjectDetails,
    changeMemberRole,
    removeMember,
    deleteProject,
    fetchProjects,
    inviteUser
} = projectsStore

const { searchUsers } = usersStore

const projectId = computed(() => route.params.projectId as string)

const tabsItems = computed(() => [
    { slot: 'general', label: t('projectSettings.tabs.general'), icon: 'i-lucide-settings' },
    { slot: 'members', label: t('projectSettings.tabs.members'), icon: 'i-lucide-users' },
    { slot: 'danger', label: t('projectSettings.tabs.danger'), icon: 'i-lucide-trash-2' }
])

const generalForm = ref<UpdateProjectDetailsRequest>({
    name: selectedProject.value?.name || '',
    prefix: selectedProject.value?.prefix || '',
    description: selectedProject.value?.description || ''
})

watch(selectedProject, (newProj) => {
    if (newProj) {
        generalForm.value.name = newProj.name
        generalForm.value.prefix = newProj.prefix
        generalForm.value.description = newProj.description || ''
    }
}, { immediate: true })

const members = ref<any[]>([])
const isMembersLoading = ref(false)

const fetchMembersList = async () => {
    if (!projectId.value) return
    isMembersLoading.value = true
    try {
        members.value = await getMembers(projectId.value)
    } catch (e) {
        console.error(e)
    } finally {
        isMembersLoading.value = false
    }
}

const availableRoles = [
    { id: 1, name: t('projectRoles.1') },
    { id: 2, name: t('projectRoles.2') },
    { id: 3, name: t('projectRoles.3') },
    { id: 4, name: t('projectRoles.4') }
]

const searchLoading = ref(false)
const searchTerm = ref('')
const searchResults = ref<UserSearchResultDto[]>([])
const selectedUserToInvite = ref<UserSearchResultDto | undefined>(undefined)
const inviteRoleId = ref<number>(3)
const isInviting = ref(false)

let debounceTimeout: ReturnType<typeof setTimeout> | null = null

watch(searchTerm, (newQuery) => {
    const trimmedQuery = newQuery?.trim()
    if (!trimmedQuery || trimmedQuery.length < 4) {
        searchResults.value = []
        if (debounceTimeout) clearTimeout(debounceTimeout)
        return
    }
    if (debounceTimeout) clearTimeout(debounceTimeout)
    searchLoading.value = true
    debounceTimeout = setTimeout(async () => {
        try {
            searchResults.value = await searchUsers(trimmedQuery, 5)
        } catch (e) {
            console.error(e)
        } finally {
            searchLoading.value = false
        }
    }, 350)
})

const handleSendInvitation = async () => {
    if (!selectedUserToInvite.value || !projectId.value) return

    isInviting.value = true
    const success = await inviteUser(projectId.value, {
        inviteeEmail: selectedUserToInvite.value.email,
        projectRoleId: inviteRoleId.value
    })

    if (success) {
        selectedUserToInvite.value = undefined
        searchResults.value = []
        await fetchMembersList()
    }
    isInviting.value = false
}

const isSavingDetails = ref(false)
const isActionProcessing = ref<string | null>(null)

const handleUpdateDetails = async () => {
    isSavingDetails.value = true
    const success = await updateProjectDetails(projectId.value, generalForm.value)
    if (success) {
        await fetchProjects()
    }
    isSavingDetails.value = false
}

const handleRoleChange = async (userId: string, roleId: number) => {
    isActionProcessing.value = userId
    await changeMemberRole(projectId.value, userId, roleId)
    await fetchMembersList()
    isActionProcessing.value = null
}
const isKickModalOpen = ref(false)
const memberToKick = ref<{ userId: string; fullName: string } | null>(null)
const isKickingMember = ref(false)

const prepareKickMember = (member: any) => {
    if (!member) return
    const lastName = member.lastName || member.lastname || ''
    memberToKick.value = {
        userId: member.userId,
        fullName: `${member.firstName || ''} ${lastName}`.trim()
    }
    isKickModalOpen.value = true
}

const confirmKickMember = async () => {
    if (!memberToKick.value || !projectId.value || isKickingMember.value) return

    const userId = memberToKick.value.userId
    isKickingMember.value = true
    isActionProcessing.value = userId

    try {
        const success = await removeMember(projectId.value, userId)
        if (success) {
            await fetchMembersList()
            isKickModalOpen.value = false
            memberToKick.value = null
        }
    } catch (e) {
        console.error("Ошибка при удалении участника:", e)
    } finally {
        isKickingMember.value = false
        isActionProcessing.value = null
    }
}
const isDeleteModalOpen = ref(false)
const deleteConfirmName = ref('')
const isDeletingProject = ref(false)

const openDeleteModal = () => {
    deleteConfirmName.value = ''
    isDeleteModalOpen.value = true
}

const confirmDeleteProject = async () => {
  if (deleteConfirmName.value !== selectedProject.value?.name || isDeletingProject.value) {
    toast.add({ title: 'Ошибка', description: 'Имя проекта введено неверно', color: 'error' })
    return
  }

  isDeletingProject.value = true

  try {
    const success = await deleteProject(projectId.value)
    if (success) {
      isDeleteModalOpen.value = false
      deleteConfirmName.value = ''
      await navigateTo('/')
      selectedProject.value = undefined
      await fetchProjects()
    }
  } catch (e) {
    console.error("Ошибка при удалении проекта:", e)
  } finally {
    isDeletingProject.value = false
  }
}

onMounted(() => {
    fetchMembersList()
})
</script>

<template>
    <div class="max-w-4xl mx-auto py-6 px-4 space-y-6">
        <div class="flex items-center gap-3">
            <UButton icon="i-lucide-arrow-left" variant="ghost" color="neutral" class="rounded-xl"
                @click="void (navigateTo('/'))"/>
            <h1 class="text-2xl font-black tracking-tight text-neutral-900 dark:text-white">
                {{ $t('projectSettings.title') }}
            </h1>
        </div>

        <UTabs :items="tabsItems" variant="link" class="w-full">
            <template #general>
                <div
                    class="mt-4 p-6 bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-2xl space-y-4 shadow-sm">
                    <form @submit.prevent="handleUpdateDetails" class="space-y-4">
                        <UFormField :label="$t('projectSettings.general.nameLabel')">
                            <UInput v-model="generalForm.name" class="w-full rounded-xl" required />
                        </UFormField>

                        <UFormField :label="$t('projectSettings.general.prefixLabel')"
                            :description="$t('projectSettings.general.prefixDesc')">
                            <UInput v-model="generalForm.prefix" class="w-full rounded-xl" required maxLength="6" />
                        </UFormField>

                        <UFormField :label="$t('projectSettings.general.descLabel')">
                            <UTextarea v-model="generalForm.description" class="w-full rounded-xl" :rows="3" />
                        </UFormField>

                        <div class="flex justify-end pt-2">
                            <UButton type="submit" color="primary" class="rounded-xl font-bold shadow-xs"
                                :loading="isSavingDetails">
                                {{ $t('projectSettings.general.saveBtn') }}
                            </UButton>
                        </div>
                    </form>
                </div>
            </template>

            <template #members>
                <div
                    class="mt-4 p-6 bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-2xl space-y-6 shadow-sm">

                    <div class="pb-6 border-b border-neutral-100 dark:border-neutral-800/60 space-y-3">
                        <h4 class="text-sm font-bold text-neutral-900 dark:text-white">
                            Пригласить нового участника
                        </h4>
                        <div class="flex flex-col sm:flex-row items-end gap-3">
                            <UFormField label="Поиск пользователя (Имя или Email)" class="flex-1 w-full">
                                <USelectMenu v-model="selectedUserToInvite" v-model:search-term="searchTerm"
                                    :items="searchResults" :loading="searchLoading" searchable clearable :filter="false"
                                    label-key="email" placeholder="Введите email или имя..." class="w-full rounded-xl">
                                    <template #item="{ item }">
                                        <div class="flex flex-col text-left py-0.5">
                                            <span class="text-xs font-bold text-neutral-950 dark:text-white">
                                                {{ item.fullName }}
                                            </span>
                                            <span class="text-[11px] text-neutral-400">
                                                {{ item.email }}
                                            </span>
                                        </div>
                                    </template>

                                    <template #empty>
                                        <div class="p-3 text-xs text-center text-neutral-400 dark:text-neutral-500">
                                            <span v-if="searchTerm.trim().length < 4">
                                                Введите минимум 4 символа для начала поиска...
                                            </span>
                                            <span v-else>
                                                Пользователи с такими данными не найдены
                                            </span>
                                        </div>
                                    </template>
                                </USelectMenu>
                            </UFormField>

                            <UFormField label="Роль в проекте" class="w-full sm:w-44">
                                <USelectMenu v-model="inviteRoleId" :items="availableRoles" value-key="id"
                                    label-key="name" class="w-full" />
                            </UFormField>

                            <UButton color="primary" icon="i-lucide-user-plus"
                                class="rounded-xl font-bold shadow-xs h-9" :disabled="!selectedUserToInvite"
                                :loading="isInviting" @click="handleSendInvitation">
                                Пригласить
                            </UButton>
                        </div>
                    </div>

                    <div class="space-y-4">
                        <div class="flex justify-between items-center">
                            <h3 class="text-base font-bold text-neutral-900 dark:text-white">
                                {{ $t('projectSettings.members.title', { count: members.length }) }}
                            </h3>
                            <UButton icon="i-lucide-refresh-cw" size="xs" variant="ghost" color="neutral"
                                @click="fetchMembersList" :loading="isMembersLoading" />
                        </div>

                        <div v-if="isMembersLoading" class="space-y-2">
                            <USkeleton class="h-14 w-full rounded-xl" v-for="i in 3" :key="i" />
                        </div>

                        <div v-else class="divide-y divide-neutral-100 dark:divide-neutral-800/60">
                            <div v-for="member in members" :key="member.userId"
                                class="py-3 flex items-center justify-between gap-4">
                                <div class="flex items-center gap-3 min-w-0">
                                    <div
                                        class="w-10 h-10 rounded-xl bg-neutral-100 dark:bg-neutral-800 font-bold text-sm flex items-center justify-center text-neutral-600 dark:text-neutral-300">
                                        {{ member.firstName ? member.firstName.charAt(0).toUpperCase() : 'U' }}
                                    </div>
                                    <div class="min-w-0">
                                        <p class="text-sm font-bold text-neutral-900 dark:text-white truncate">
                                            {{ member.firstName }} {{ member.lastName || member.lastname }}
                                        </p>
                                        <p class="text-xs text-neutral-400 truncate">{{ member.email }}</p>
                                    </div>
                                </div>
                                <div class="flex items-center gap-2 shrink-0">
                                    <template v-if="member.userId !== selectedProject?.creatorId">
                                        <USelectMenu :model-value="member.projectRoleId" :items="availableRoles"
                                            value-key="id" label-key="name"
                                            :disabled="isActionProcessing === member.userId"
                                            @update:model-value="(roleId: any) => handleRoleChange(member.userId, roleId)"
                                            class="w-40" size="sm" />
                                            
                                        <UModal v-model:open="isKickModalOpen">
                                            <template #default>
                                                <UButton color="error" variant="soft" icon="i-lucide-user-x" size="sm"
                                                    class="rounded-lg" :loading="isActionProcessing === member.userId"
                                                    @click="prepareKickMember(member)" />
                                            </template>

                                            <template #content>
                                                <div class="p-6 space-y-4 w-full max-w-md bg-white dark:bg-neutral-950 rounded-2xl border border-neutral-200 dark:border-neutral-800">
                                                    <div>
                                                        <h3 class="text-lg font-bold text-neutral-900 dark:text-white">
                                                            {{ $t('projectSettings.modals.kick.title') }}
                                                        </h3>
                                                        <p class="text-xs text-neutral-400 dark:text-neutral-500 mt-1">
                                                            {{ $t('projectSettings.modals.kick.desc', { name: memberToKick?.fullName }) }}
                                                        </p>
                                                    </div>

                                                    <div class="flex justify-end gap-2 pt-2 border-t border-neutral-100 dark:border-neutral-800">
                                                        <UButton color="neutral" variant="ghost" class="rounded-xl font-bold"
                                                            @click="void (isKickModalOpen = false)">
                                                            {{ $t('projectSettings.modals.cancel') }}
                                                        </UButton>
                                                        <UButton color="error" class="rounded-xl font-bold shadow-xs" :loading="isKickingMember"
                                                            @click="confirmKickMember">
                                                            {{ $t('projectSettings.modals.kick.confirm') }}
                                                        </UButton>
                                                    </div>
                                                </div>
                                            </template>
                                        </UModal>
                                    </template>
                                    <template v-else>
                                        <UBadge color="error" variant="subtle" class="font-bold rounded-lg">
                                            {{ $t('projectSettings.members.creatorBadge') }}
                                        </UBadge>
                                    </template>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </template>

            <template #danger>
                <div
                    class="mt-4 p-6 bg-white dark:bg-neutral-900 border border-error-200 dark:border-error-900/60 rounded-2xl space-y-4 shadow-sm">
                    <h3 class="text-base font-bold text-error-600 dark:text-error-400">
                        {{ $t('projectSettings.danger.title') }}
                    </h3>
                    <p class="text-sm text-neutral-500 dark:text-neutral-400">
                        {{ $t('projectSettings.danger.desc') }}
                    </p>
                    <div class="pt-2">
                        <UModal v-model:open="isDeleteModalOpen">
                            <template #default>
                                <UButton color="error" icon="i-lucide-trash-2" class="rounded-xl font-bold shadow-xs"
                                    @click="openDeleteModal">
                                    {{ $t('projectSettings.danger.deleteBtn') }}
                                </UButton>
                            </template>

                            <template #content>
                                <div class="p-6 space-y-4 w-full max-w-md bg-white dark:bg-neutral-950 rounded-2xl border border-neutral-200 dark:border-neutral-800 text-left">
                                    <div class="flex gap-3 items-start">
                                        <div class="p-2 bg-error-50 dark:bg-error-950/50 rounded-xl text-error-600 shrink-0">
                                            <UIcon name="i-lucide-alert-triangle" class="w-5 h-5" />
                                        </div>
                                        <div>
                                            <h3 class="text-lg font-bold text-neutral-900 dark:text-white">
                                                {{ $t('projectSettings.modals.delete.title') }}
                                            </h3>
                                            <p class="text-xs text-neutral-400 dark:text-neutral-500 mt-1">
                                                {{ $t('projectSettings.modals.delete.desc') }}
                                            </p>
                                        </div>
                                    </div>

                                    <div class="space-y-2 pt-1">
                                        <label class="text-xs font-medium text-neutral-600 dark:text-neutral-400 block">
                                            {{ $t('projectSettings.modals.delete.inputLabel') }}
                                            <span class="font-mono font-bold text-neutral-900 dark:text-white">
                                                {{ selectedProject?.name }}
                                            </span>
                                        </label>
                                        <UInput v-model="deleteConfirmName"
                                            :placeholder="$t('projectSettings.modals.delete.inputPlaceholder')" class="w-full rounded-xl"
                                            @keydown.enter="confirmDeleteProject" />
                                    </div>

                                    <div class="flex justify-end gap-2 pt-2 border-t border-neutral-100 dark:border-neutral-800">
                                        <UButton color="neutral" variant="ghost" class="rounded-xl font-bold"
                                            @click="void (isDeleteModalOpen = false)">
                                            {{ $t('projectSettings.modals.cancel') }}
                                        </UButton>
                                        <UButton color="error" class="rounded-xl font-bold shadow-xs"
                                            :disabled="deleteConfirmName !== selectedProject?.name" :loading="isDeletingProject"
                                            @click="confirmDeleteProject">
                                            {{ $t('projectSettings.modals.delete.confirm') }}
                                        </UButton>
                                    </div>
                                </div>
                            </template>
                        </UModal>
                    </div>
                </div>
            </template>
        </UTabs>
    </div>
</template>