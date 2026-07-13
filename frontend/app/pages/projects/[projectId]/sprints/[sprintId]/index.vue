<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { SprintStatus } from '~/types/sprint'

const route = useRoute()
const { t } = useI18n()
const projectId = computed(() => route.params.projectId as string)
const sprintId = computed(() => route.params.sprintId as string)

const sprintStore = useSprintStore()
const issuesStore = useIssuesStore()

const { currentSprint, isLoadingDetail, isSubmittingAction } = storeToRefs(sprintStore)
const { issuesList, isLoadingList } = storeToRefs(issuesStore)

const isAddIssuesModalOpen = ref(false)
const selectedIssuesForSprint = ref<string[]>([])

const loadData = async () => {
    await sprintStore.fetchSprintDetails(projectId.value, sprintId.value)
}

onMounted(() => {
    loadData()
})

const getSprintStatusConfig = (status: SprintStatus) => {
    const configs = {
        [SprintStatus.Planning]: { label: 'Планирование', color: 'neutral' as const, icon: 'i-lucide-calendar-days' },
        [SprintStatus.Active]: { label: 'Активен', color: 'primary' as const, icon: 'i-lucide-play' },
        [SprintStatus.Completed]: { label: 'Завершен', color: 'success' as const, icon: 'i-lucide-check-circle' },
        [SprintStatus.Cancelled]: { label: 'Отменен', color: 'error' as const, icon: 'i-lucide-x-circle' },
    }
    return configs[status] || { label: status, color: 'neutral' as const, icon: 'i-lucide-help-circle' }
}

const formatDate = (dateStr: string) => {
    if (!dateStr) return '—'
    return new Date(dateStr).toLocaleDateString('ru-RU', { day: '2-digit', month: 'short', year: 'numeric' })
}

const handleStartSprint = async () => {
    await sprintStore.startSprint(projectId.value, sprintId.value)
}

const handleCompleteSprint = async () => {
    await sprintStore.completeSprint(projectId.value, sprintId.value)
}

const handleCancelSprint = async () => {
    if (confirm('Вы уверены, что хотите отменить этот спринт? Все незавершенные задачи вернутся в бэклог.')) {
        await sprintStore.cancelSprint(projectId.value, sprintId.value)
    }
}

const handleRemoveIssue = async (issueId: string) => {
    await sprintStore.removeIssueFromSprint(projectId.value, sprintId.value, issueId)
}

const openAddIssuesModal = async () => {
    selectedIssuesForSprint.value = []
    isAddIssuesModalOpen.value = true
    await issuesStore.fetchIssues(projectId.value, { pageSize: 50, pageNumber: 1 })
}

const closeAddIssuesModal = () => {
    isAddIssuesModalOpen.value = false
    selectedIssuesForSprint.value = []
}

const handleAddIssuesConfirm = async () => {
    if (selectedIssuesForSprint.value.length === 0) return

    const success = await sprintStore.addIssuesToSprint(
        projectId.value,
        sprintId.value,
        selectedIssuesForSprint.value
    )
    if (success) {
        closeAddIssuesModal()
    }
}

const availableIssuesForModal = computed(() => {
    if (!currentSprint.value) return issuesList.value
    const existingIds = new Set(currentSprint.value.issues.map(i => i.id))
    return issuesList.value.filter(issue => !existingIds.has(issue.id))
})
</script>

<template>
    <div class="space-y-6 max-w-7xl mx-auto px-4 sm:px-6">
        <div
            class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 border-b border-neutral-100 dark:border-neutral-800">
            <div class="flex items-center gap-3">
                <UButton :to="`/projects/${projectId}/sprints`" icon="i-lucide-arrow-left" color="neutral"
                    variant="ghost" class="rounded-xl" />
                <div>
                    <div class="flex items-center gap-2 mb-1">
                        <span class="text-xs font-bold text-neutral-400">СПРИНТ</span>
                        <UBadge v-if="currentSprint" :color="getSprintStatusConfig(currentSprint.status).color"
                            size="xs" variant="subtle" class="flex items-center gap-1 rounded-md">
                            <UIcon :name="getSprintStatusConfig(currentSprint.status).icon" class="w-3 h-3" />
                            {{ getSprintStatusConfig(currentSprint.status).label }}
                        </UBadge>
                    </div>
                    <h1 class="text-2xl font-black tracking-tight text-neutral-900 dark:text-white">
                        {{ currentSprint?.name ?? 'Загрузка спринта...' }}
                    </h1>
                </div>
            </div>

            <div v-if="currentSprint" class="flex items-center gap-2">
                <UButton v-if="currentSprint.status === SprintStatus.Planning" icon="i-lucide-play" color="primary"
                    class="rounded-xl font-bold" :loading="isSubmittingAction" @click="() => handleStartSprint()">
                    Запустить спринт
                </UButton>

                <UButton v-if="currentSprint.status === SprintStatus.Active" icon="i-lucide-check-check" color="success"
                    class="rounded-xl font-bold" :loading="isSubmittingAction" @click="() => handleCompleteSprint()">
                    Завершить спринт
                </UButton>

                <UButton
                    v-if="currentSprint.status === SprintStatus.Planning || currentSprint.status === SprintStatus.Active"
                    icon="i-lucide-plus" color="neutral" variant="soft" class="rounded-xl font-bold"
                    @click="() => openAddIssuesModal()">
                    Добавить задачи
                </UButton>

                <UButton
                    v-if="currentSprint.status === SprintStatus.Planning || currentSprint.status === SprintStatus.Active"
                    icon="i-lucide-ban" color="error" variant="ghost" class="rounded-xl font-bold"
                    :loading="isSubmittingAction" @click="handleCancelSprint">
                    Отменить спринт
                </UButton>
            </div>
        </div>

        <div v-if="isLoadingDetail" class="space-y-4">
            <USkeleton class="h-32 rounded-2xl" />
            <USkeleton class="h-64 rounded-2xl" />
        </div>

        <div v-else-if="currentSprint" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <div class="lg:col-span-2 space-y-6">
                <UCard class="rounded-2xl shadow-sm">
                    <template #header>
                        <h2 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
                            <UIcon name="i-lucide-target" class="w-4 h-4" /> Цель спринта
                        </h2>
                    </template>
                    <p class="text-sm text-neutral-700 dark:text-neutral-300 whitespace-pre-line leading-relaxed">
                        {{ currentSprint.goal || 'Цель для этого спринта не сформулирована.' }}
                    </p>
                </UCard>

                <UCard class="rounded-2xl shadow-sm">
                    <template #header>
                        <div class="flex justify-between items-center">
                            <h2
                                class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
                                <UIcon name="i-lucide-list-todo" class="w-4 h-4" /> Задачи спринта ({{
                                    currentSprint.issues.length }})
                            </h2>
                        </div>
                    </template>

                    <div v-if="currentSprint.issues.length === 0" class="text-center py-8 text-neutral-400 text-sm">
                        В этом спринте пока нет задач. Нажмите кнопку «Добавить задачи», чтобы наполнить спринт.
                    </div>

                    <div v-else class="divide-y divide-neutral-100 dark:divide-neutral-800">
                        <div v-for="issue in currentSprint.issues" :key="issue.id"
                            class="py-3 flex items-center justify-between gap-4 hover:bg-neutral-50/50 dark:hover:bg-neutral-900/30 px-2 transition rounded-xl group/item">
                            <div class="flex items-center gap-3 min-w-0">
                                <span class="font-mono text-xs font-bold text-neutral-400 shrink-0">TASK-{{ issue.key
                                }}</span>
                                <NuxtLink :to="`/projects/${projectId}/issues/${issue.id}`"
                                    class="text-sm font-medium text-neutral-800 dark:text-neutral-200 hover:text-primary dark:hover:text-primary truncate">
                                    {{ issue.title }}
                                </NuxtLink>
                            </div>

                            <div class="flex items-center gap-3 shrink-0">
                                <UBadge size="xs" variant="outline" color="neutral" class="rounded-md">
                                    {{ issue.status }}
                                </UBadge>
                                <UBadge v-if="issue.storyPoints" size="xs" color="primary" variant="subtle"
                                    class="font-bold rounded-md">
                                    {{ issue.storyPoints }} SP
                                </UBadge>
                                <span class="text-xs text-neutral-400 max-w-[120px] truncate"
                                    :title="issue.assigneeName || 'Без исполнителя'">
                                    {{ issue.assigneeName || '—' }}
                                </span>

                                <UButton
                                    v-if="currentSprint.status === SprintStatus.Planning || currentSprint.status === SprintStatus.Active"
                                    icon="i-lucide-x" color="error" variant="ghost" size="xs"
                                    class="opacity-0 group-hover/item:opacity-100 transition rounded-md"
                                    :loading="isSubmittingAction" title="Удалить из спринта"
                                    @click.stop="handleRemoveIssue(issue.id)" />
                            </div>
                        </div>
                    </div>
                </UCard>
            </div>

            <aside class="space-y-4">
                <UCard class="rounded-2xl shadow-sm sticky top-6">
                    <template #header>
                        <h2 class="text-sm font-bold text-neutral-900 dark:text-white flex items-center gap-2">
                            <UIcon name="i-lucide-sliders-horizontal" class="w-4 h-4 text-neutral-400" /> Параметры и
                            Емкость
                        </h2>
                    </template>

                    <div class="space-y-4">
                        <div class="bg-neutral-50 dark:bg-neutral-900/60 p-3 rounded-xl space-y-2 text-xs">
                            <div class="flex justify-between">
                                <span class="text-neutral-400">Дата начала:</span>
                                <span class="font-medium text-neutral-800 dark:text-neutral-200">{{
                                    formatDate(currentSprint.startDate) }}</span>
                            </div>
                            <div class="flex justify-between">
                                <span class="text-neutral-400">Дата окончания:</span>
                                <span class="font-medium text-neutral-800 dark:text-neutral-200">{{
                                    formatDate(currentSprint.endDate) }}</span>
                            </div>
                        </div>

                        <div class="grid grid-cols-3 gap-2 text-center">
                            <div class="bg-neutral-50 dark:bg-neutral-900 p-2.5 rounded-xl">
                                <span class="text-[10px] text-neutral-400 block mb-0.5">Емкость</span>
                                <span class="text-base font-bold text-neutral-800 dark:text-neutral-200">{{
                                    currentSprint.capacity
                                }}</span>
                            </div>
                            <div class="bg-neutral-50 dark:bg-neutral-900 p-2.5 rounded-xl">
                                <span class="text-[10px] text-neutral-400 block mb-0.5">Взято</span>
                                <span class="text-base font-bold text-primary">{{ currentSprint.committedPoints
                                }}</span>
                            </div>
                            <div class="bg-neutral-50 dark:bg-neutral-900 p-2.5 rounded-xl">
                                <span class="text-[10px] text-neutral-400 block mb-0.5">Закрыто</span>
                                <span class="text-base font-bold text-success">{{ currentSprint.completedPoints
                                }}</span>
                            </div>
                        </div>

                        <div class="space-y-1">
                            <div class="flex justify-between text-xs">
                                <span class="text-neutral-400">Загрузка емкости:</span>
                                <span class="font-bold"
                                    :class="currentSprint.committedPoints > currentSprint.capacity ? 'text-error' : 'text-neutral-600'">
                                    {{ Math.round((currentSprint.committedPoints / currentSprint.capacity) * 100) }}%
                                </span>
                            </div>
                            <UProgress :model-value="currentSprint.committedPoints" :max="currentSprint.capacity"
                                :color="currentSprint.committedPoints > currentSprint.capacity ? 'error' : 'primary'"
                                size="sm" class="rounded-full" />
                        </div>
                    </div>
                </UCard>
            </aside>
        </div>

        <UModal v-model:open="isAddIssuesModalOpen">
            <template #content>
                <UCard class="rounded-2xl max-w-lg w-full mx-auto">
                    <template #header>
                        <div class="flex justify-between items-center">
                            <h3 class="text-base font-bold text-neutral-900 dark:text-white">Добавление задач в спринт
                            </h3>
                            <UButton color="neutral" variant="ghost" icon="i-lucide-x" @click="closeAddIssuesModal" />
                        </div>
                    </template>

                    <div class="space-y-4 max-h-[400px] overflow-y-auto">
                        <div v-if="isLoadingList" class="space-y-2 py-4">
                            <USkeleton class="h-10 rounded-xl" v-for="i in 3" :key="i" />
                        </div>

                        <div v-else-if="availableIssuesForModal.length === 0"
                            class="text-center py-6 text-sm text-neutral-400">
                            Нет доступных свободных задач для добавления.
                        </div>

                        <div v-else class="space-y-2">
                            <div v-for="issue in availableIssuesForModal" :key="issue.id"
                                class="flex items-center gap-3 p-3 border border-neutral-100 dark:border-neutral-800 rounded-xl hover:bg-neutral-50 dark:hover:bg-neutral-900/50 transition">
                                <UCheckbox :model-value="selectedIssuesForSprint.includes(issue.id)"
                                    :name="`issue-${issue.id}`" color="primary" @update:model-value="(checked) => {
                                        if (checked) {
                                            selectedIssuesForSprint.push(issue.id)
                                        } else {
                                            selectedIssuesForSprint = selectedIssuesForSprint.filter(id => id !== issue.id)
                                        }
                                    }" @click.stop />
                                <div class="min-w-0 flex-1">
                                    <div class="flex items-center gap-1.5 mb-0.5">
                                        <span class="font-mono text-[11px] font-bold text-neutral-400">TASK-{{
                                            issue.number }}</span>
                                        <UBadge size="xs" color="neutral" variant="subtle">{{ issue.status }}</UBadge>
                                    </div>
                                    <p class="text-sm font-medium text-neutral-800 dark:text-neutral-200 truncate">{{
                                        issue.title }}</p>
                                </div>

                                <span v-if="issue.storyPoints"
                                    class="text-xs font-bold text-neutral-500 bg-neutral-100 dark:bg-neutral-800 px-2 py-1 rounded-md shrink-0">
                                    {{ issue.storyPoints }} SP
                                </span>
                            </div>
                        </div>
                    </div>

                    <template #footer>
                        <div class="flex justify-end gap-2">
                            <UButton color="neutral" variant="ghost" class="rounded-xl" @click="closeAddIssuesModal">
                                Отмена
                            </UButton>
                            <UButton color="primary" class="rounded-xl font-bold"
                                :disabled="!selectedIssuesForSprint || selectedIssuesForSprint.length === 0"
                                :loading="isSubmittingAction" @click="handleAddIssuesConfirm">
                                Добавить выбранные ({{ selectedIssuesForSprint?.length ?? 0 }})
                            </UButton>
                        </div>
                    </template>
                </UCard>
            </template>
        </UModal>
    </div>
</template>