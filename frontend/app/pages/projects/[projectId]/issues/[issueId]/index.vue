<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { IssueStatus } from '~/types/issue'
import { getStatusConfig, getPriorityConfig, getTypeConfig } from '~/utils/IssueHelpers'

const { t } = useI18n()
const route = useRoute()
const projectId = computed(() => route.params.projectId as string)
const issueId = computed(() => route.params.issueId as string)

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()
const { currentIssue, isLoadingDetail, isAddingComment } = storeToRefs(issuesStore)

const isUpdatingStatus = ref(false)
const isUpdatingAssignee = ref(false)

const statusItems = Object.keys(IssueStatus)
  .filter(key => isNaN(Number(key))) 
  .map(key => {
    const value = IssueStatus[key as keyof typeof IssueStatus]
    return {
      value: value,
      label: getStatusConfig(value).label
    }
  })
const selectedStatus = ref<IssueStatus>(0)
const selectedAssigneeId = ref<string | null>(null)

const getSeverityLabel = (severity: number | null) => {
  if (severity === null) return '—'
  const keys: Record<number, string> = {
    0: 'issues.severityLevels.s4',
    1: 'issues.severityLevels.s3',
    2: 'issues.severityLevels.s2',
    3: 'issues.severityLevels.s1'
  }
  return keys[severity] ? t(keys[severity]) : t('issues.severityLevels.unknown', { level: severity })
}

const assigneeOptions = ref<{ label: string; value: string | null }[]>([
  { label: t('issues.assigneeNone'), value: null }
])

const loadIssue = () => issuesStore.fetchIssueById(issueId.value)

const loadMembers = async () => {
  const members = await projectsStore.getMembers(projectId.value)
  assigneeOptions.value = [
    { label: t('issues.assigneeNone'), value: null },
    ...members.map(m => ({
      label: `${m.firstName} ${m.lastname || ''}`,
      value: m.userId
    }))
  ]
}

const formatDate = (dateStr: string | null | undefined) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

watch(currentIssue, (issue) => {
  if (!issue) return
  selectedStatus.value = issue.status
  selectedAssigneeId.value = issue.assigneeId
}, { immediate: true })

onMounted(async () => {
  await Promise.all([loadIssue(), loadMembers()])
})

watch(issueId, async () => {
  await loadIssue()
})

const handleStatusChange = async () => {
  if (selectedStatus.value === currentIssue.value?.status) return
  isUpdatingStatus.value = true
  await issuesStore.changeStatus(issueId.value, selectedStatus.value)
  isUpdatingStatus.value = false
}

const handleAssigneeChange = async () => {
  if (selectedAssigneeId.value === currentIssue.value?.assigneeId) return
  isUpdatingAssignee.value = true
  await issuesStore.assign(issueId.value, selectedAssigneeId.value)
  isUpdatingAssignee.value = false
}

const commentText = ref('')
const commentIsInternal = ref(false)

const handleAddComment = async () => {
  if (!commentText.value.trim()) return

  const success = await issuesStore.postComment(
    issueId.value,
    commentText.value,
    commentIsInternal.value
  )

  if (success) {
    commentText.value = ''
    commentIsInternal.value = false
  }
}
</script>

<template>
  <div class="space-y-6 max-w-7xl mx-auto px-4 sm:px-6">
    <div class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4 pb-4 border-b border-neutral-100 dark:border-neutral-800">
      <div class="flex items-start gap-3 min-w-0">
        <UButton :to="`/projects/${projectId}/issues`" icon="i-lucide-arrow-left" color="neutral" variant="ghost" class="rounded-xl shrink-0 mt-1" />
        <div class="min-w-0">
          <div class="flex items-center gap-2 flex-wrap mb-1">
            <span class="text-xs font-mono font-bold text-neutral-400">TASK-{{ currentIssue?.number ?? '…' }}</span>
            <span v-if="currentIssue" class="text-xs text-neutral-400 flex items-center gap-1">
              <UIcon name="i-lucide-eye" class="w-3 h-3" /> {{ currentIssue.viewCount }}
            </span>
          </div>
          <div class="flex items-center gap-2 min-w-0">
            <UIcon 
              v-if="currentIssue" 
              :name="getTypeConfig(currentIssue.type).icon"
              :color="getTypeConfig(currentIssue.type).color"
              class="w-5 h-5 shrink-0 mt-1" 
            />
            <h1 class="text-2xl font-black tracking-tight text-neutral-900 dark:text-white truncate">
              {{ currentIssue?.title ?? $t('issues.loading') }}
            </h1>
          </div>
        </div>
      </div>

      <UButton v-if="currentIssue" :to="`/projects/${projectId}/issues/${issueId}/edit`" icon="i-lucide-pencil" color="neutral" variant="soft" class="rounded-xl font-bold shrink-0 w-full sm:w-auto justify-center">
        {{ $t('issues.editBtn') }}
      </UButton>
    </div>

    <div v-if="isLoadingDetail" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 space-y-4">
        <USkeleton class="h-48 rounded-2xl" />
        <USkeleton class="h-32 rounded-2xl" />
      </div>
      <USkeleton class="h-[500px] rounded-2xl" />
    </div>

    <div v-else-if="currentIssue" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 space-y-6">
        <UAlert v-if="currentIssue.isBlocked" icon="i-lucide-octagon-alert" color="error" variant="soft" :title="$t('issues.blockedAlertTitle')" :description="currentIssue.blockReason || $t('issues.blockedAlertDefaultDesc')" class="rounded-xl" />

        <UCard class="rounded-2xl shadow-sm">
          <template #header>
            <h2 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
              <UIcon name="i-lucide-align-left" class="w-4 h-4" /> {{ $t('issues.description') }}
            </h2>
          </template>
          <p class="text-sm text-neutral-700 dark:text-neutral-300 whitespace-pre-line leading-relaxed">
            {{ currentIssue.description || $t('issues.descriptionEmpty') }}
          </p>
        </UCard>

        <UCard v-if="currentIssue.stepsToReproduce" class="rounded-2xl shadow-sm border border-amber-100 dark:border-amber-950/40">
          <template #header>
            <h2 class="text-xs font-bold uppercase tracking-wider text-amber-500 flex items-center gap-2">
              <UIcon name="i-lucide-list-ordered" class="w-4 h-4" /> {{ $t('issues.stepsToReproduce') }}
            </h2>
          </template>
          <p class="text-sm text-neutral-700 dark:text-neutral-300 whitespace-pre-line font-mono bg-neutral-50 dark:bg-neutral-900/50 p-3 rounded-xl border border-neutral-100 dark:border-neutral-800">
            {{ currentIssue.stepsToReproduce }}
          </p>
          <div class="mt-3 flex items-center gap-2 text-xs text-neutral-400">
            <span>{{ $t('issues.reproducibleStable') }}</span>
            <UBadge :color="currentIssue.isReproducible ? 'error' : 'neutral'" size="xs" variant="subtle">
              {{ currentIssue.isReproducible ? $t('issues.yes') : $t('issues.no') }}
            </UBadge>
          </div>
        </UCard>

        <UCard class="rounded-2xl shadow-sm">
          <template #header>
            <h2 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
              <UIcon name="i-lucide-hourglass" class="w-4 h-4" /> {{ $t('issues.timeTracking') }}
            </h2>
          </template>
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
            <div class="bg-neutral-50 dark:bg-neutral-900 p-3 rounded-xl text-center">
              <span class="text-xs text-neutral-400 block mb-1">Story Points</span>
              <span class="text-lg font-bold text-neutral-800 dark:text-neutral-200">{{ currentIssue.storyPoints ?? '—' }}</span>
            </div>
            <div class="bg-neutral-50 dark:bg-neutral-900 p-3 rounded-xl text-center">
              <span class="text-xs text-neutral-400 block mb-1">{{ $t('issues.timeEstimation') }}</span>
              <span class="text-lg font-bold text-neutral-800 dark:text-neutral-200">{{ currentIssue.estimatedHours ?? '—' }}</span>
            </div>
            <div class="bg-neutral-50 dark:bg-neutral-900 p-3 rounded-xl text-center">
              <span class="text-xs text-neutral-400 block mb-1">{{ $t('issues.timeLogged') }}</span>
              <span class="text-lg font-bold text-emerald-600 dark:text-emerald-400">{{ currentIssue.loggedHours ?? '0' }}</span>
            </div>
            <div class="bg-neutral-50 dark:bg-neutral-900 p-3 rounded-xl text-center">
              <span class="text-xs text-neutral-400 block mb-1">{{ $t('issues.timeRemaining') }}</span>
              <span class="text-lg font-bold text-neutral-800 dark:text-neutral-200">{{ currentIssue.remainingHours ?? '—' }}</span>
            </div>
          </div>
        </UCard>

        <UCard v-if="currentIssue.links.length" class="rounded-2xl shadow-sm">
          <template #header>
            <h2 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
              <UIcon name="i-lucide-link" class="w-4 h-4" /> {{ $t('issues.linkedIssues') }}
            </h2>
          </template>
          <div class="flex flex-col gap-2">
            <div v-for="link in currentIssue.links" :key="link.id" class="flex items-center justify-between text-xs bg-neutral-50 dark:bg-neutral-900 p-3 rounded-xl border border-neutral-100 dark:border-neutral-800/50 hover:bg-neutral-100/50 transition">
              <div class="flex items-center gap-2 min-w-0">
                <span class="font-mono font-bold text-primary shrink-0">TASK-{{ link.targetIssueNumber }}</span>
                <span class="text-neutral-700 dark:text-neutral-300 truncate">{{ link.targetIssueTitle }}</span>
              </div>
              <UBadge size="xs" variant="outline" color="neutral" class="rounded-md">{{ link.linkType }}</UBadge>
            </div>
          </div>
        </UCard>

        <div class="space-y-4">
          <UCard class="rounded-2xl shadow-sm">
            <template #header>
              <h2 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
                <UIcon name="i-lucide-message-square" class="w-4 h-4" />
                {{ $t('issues.commentsTitle', { count: currentIssue.comments.length }) }}
              </h2>
            </template>

            <div v-if="currentIssue.comments.length === 0" class="text-sm text-neutral-400 py-4 text-center">
              {{ $t('issues.commentsEmpty') }}
            </div>
            <div v-else class="flex flex-col gap-4">
              <div v-for="comment in currentIssue.comments" :key="comment.id" class="border border-neutral-100 dark:border-neutral-800/60 p-4 rounded-xl flex flex-col gap-2">
                <div class="flex justify-between items-center">
                  <div class="flex items-center gap-2">
                    <UAvatar :alt="comment.authorName" size="xs" class="font-bold" />
                    <span class="text-xs font-semibold text-neutral-800 dark:text-neutral-200">{{ comment.authorName }}</span>
                  </div>
                  <span class="text-[10px] text-neutral-400 font-mono">
                    {{ formatDate(comment.createdAt) }}
                  </span>
                </div>
                <p class="text-sm text-neutral-700 dark:text-neutral-300 ml-8 whitespace-pre-line">{{ comment.text }}</p>
              </div>
            </div>
          </UCard>

          <UCard class="rounded-2xl shadow-sm mt-4">
            <template #header>
              <h3 class="text-xs font-bold uppercase tracking-wider text-neutral-400 flex items-center gap-2">
                <UIcon name="i-lucide-pen-line" class="w-4 h-4" /> {{ $t('issues.writeComment') }}
              </h3>
            </template>

            <div class="space-y-4">
              <UTextarea v-model="commentText" :placeholder="$t('issues.commentPlaceholder')" :rows="4" autoresize size="md" class="w-full" />
              <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 pt-2">
                <label class="flex items-center gap-2 text-xs text-neutral-500 cursor-pointer select-none">
                  <input type="checkbox" v-model="commentIsInternal" class="rounded text-primary border-neutral-300 focus:ring-primary h-4 w-4" />
                  <span>{{ $t('issues.internalCommentLabel') }}</span>
                </label>
                <UButton icon="i-lucide-send" color="primary" class="rounded-xl font-bold justify-center px-5" :disabled="!commentText.trim()" :loading="isAddingComment" @click="handleAddComment">
                  {{ $t('issues.sendCommentBtn') }}
                </UButton>
              </div>
            </div>
          </UCard>
        </div>
      </div>

      <aside class="space-y-4">
        <UCard class="rounded-2xl shadow-sm sticky top-6">
          <template #header>
            <h2 class="text-sm font-bold text-neutral-900 dark:text-white flex items-center gap-2">
              <UIcon name="i-lucide-sliders-horizontal" class="w-4 h-4 text-neutral-400" /> {{ $t('issues.issueDetails') }}
            </h2>
          </template>

          <div class="space-y-4">
            <UFormField :label="$t('issues.fieldStatus')">
              <div class="flex gap-2 w-full">
                <USelectMenu v-model="selectedStatus" :items="statusItems" value-key="value" class="flex-1" />
                <UButton icon="i-lucide-check" color="primary" variant="soft" class="rounded-xl shrink-0" :loading="isUpdatingStatus" @click="handleStatusChange" />
              </div>
            </UFormField>

            <UFormField :label="$t('issues.fieldAssignee')">
              <div class="flex gap-2 w-full">
                <USelectMenu v-model="selectedAssigneeId" :items="assigneeOptions" value-key="value" class="flex-1" />
                <UButton icon="i-lucide-user-check" color="primary" variant="soft" class="rounded-xl shrink-0" :loading="isUpdatingAssignee" @click="handleAssigneeChange" />
              </div>
            </UFormField>

            <div class="pt-4 border-t border-neutral-100 dark:border-neutral-800 space-y-3 text-xs">
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldType') }}</span>
                <span class="font-medium flex items-center gap-1">
                  <UIcon :name="getTypeConfig(currentIssue.type).icon" :color="getTypeConfig(currentIssue.type).color" class="w-3.5 h-3.5" />
                  {{ getTypeConfig(currentIssue.type).label }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldPriority') }}</span>
                <UBadge :color="getPriorityConfig(currentIssue.priority).color" size="xs" variant="subtle" class="rounded-md flex items-center gap-1">
                  <UIcon :name="getPriorityConfig(currentIssue.priority).icon" class="w-3 h-3" />
                  {{ getPriorityConfig(currentIssue.priority).label }}
                </UBadge>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldSeverity') }}</span>
                <span class="font-medium text-neutral-800 dark:text-neutral-200">
                  {{ getSeverityLabel(currentIssue.severity) }}
                </span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldReporter') }}</span>
                <span class="font-medium text-neutral-800 dark:text-neutral-200">{{ currentIssue.reporterName }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldComponent') }}</span>
                <span class="font-medium font-mono text-neutral-600 dark:text-neutral-400">{{ currentIssue.component || '—' }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldVersion') }}</span>
                <span class="font-medium font-mono text-neutral-600 dark:text-neutral-400">{{ currentIssue.version || '—' }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldFixVersion') }}</span>
                <span class="font-medium font-mono text-neutral-600 dark:text-neutral-400">{{ currentIssue.fixVersion || '—' }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-neutral-400">{{ $t('issues.fieldEnvironment') }}</span>
                <span class="font-medium text-neutral-600 dark:text-neutral-400 truncate max-w-[150px]" :title="currentIssue.environment || ''">
                  {{ currentIssue.environment || '—' }}
                </span>
              </div>
            </div>

            <div v-if="currentIssue.tags && currentIssue.tags.length" class="pt-3 border-t border-neutral-100 dark:border-neutral-800">
              <span class="text-[11px] font-bold text-neutral-400 uppercase tracking-wider block mb-2">{{ $t('issues.fieldTags') }}</span>
              <div class="flex flex-wrap gap-1">
                <UBadge v-for="tag in currentIssue.tags" :key="tag" variant="subtle" color="neutral" size="xs" class="rounded-md">
                  {{ tag }}
                </UBadge>
              </div>
            </div>

            <div class="pt-3 border-t border-neutral-100 dark:border-neutral-800 space-y-2 text-[11px] font-mono text-neutral-400">
              <div class="flex justify-between">
                <span>{{ $t('issues.created') }}</span>
                <span>{{ formatDate(currentIssue.createdAt) }}</span>
              </div>
              <div class="flex justify-between">
                <span>{{ $t('issues.updated') }}</span>
                <span>{{ formatDate(currentIssue.updatedAt) }}</span>
              </div>
              <div v-if="currentIssue.dueDate" class="flex justify-between text-warning-500">
                <span>{{ $t('issues.deadline') }}</span>
                <span>{{ formatDate(currentIssue.dueDate) }}</span>
              </div>
              <div v-if="currentIssue.startedAt" class="flex justify-between">
                <span>{{ $t('issues.startedAt') }}</span>
                <span>{{ formatDate(currentIssue.startedAt) }}</span>
              </div>
              <div v-if="currentIssue.resolvedAt" class="flex justify-between text-emerald-500">
                <span>{{ $t('issues.resolvedAt') }}</span>
                <span>{{ formatDate(currentIssue.resolvedAt) }}</span>
              </div>
              <div v-if="currentIssue.closedAt" class="flex justify-between">
                <span>{{ $t('issues.closedAt') }}</span>
                <span>{{ formatDate(currentIssue.closedAt) }}</span>
              </div>
            </div>
          </div>
        </UCard>
      </aside>
    </div>
  </div>
</template>