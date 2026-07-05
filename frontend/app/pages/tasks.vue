<script setup lang="ts">
import { useIssuesStore } from '~/stores/issues'
import { useProjectsStore } from '~/stores/projects'
import { useAuth } from '~/composables/useAuth'
import { getStatusConfig, getPriorityConfig, getTypeConfig } from '~/utils/IssueHelpers' 

const { t } = useI18n()
const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()
const { user } = useAuth()

const formatDate = (dateStr: string) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const handleIssueClick = async (projectId: string, issueId: string) => {
  await projectsStore.setCurrentProject(projectId)
  await navigateTo(`/projects/${projectId}/issues/${issueId}`)
}

onMounted(() => {
  if (user.value?.id) {
    issuesStore.fetchUserActiveIssues(user.value.id)
  }
})
</script>

<template>
    <div class="py-6 max-w-6xl mx-auto px-4">
        <div class="mb-6 flex justify-between items-center">
            <div>
                <h1 class="text-2xl font-bold tracking-tight text-neutral-900 dark:text-white">
                    {{ $t('issues.myActiveTitle') }}
                </h1>
                <p class="text-sm text-neutral-500 dark:text-neutral-400 mt-1">
                    {{ $t('issues.myActiveSubtitle') }}
                </p>
            </div>
            <UBadge size="lg" variant="subtle" color="primary" class="font-semibold">
                {{ $t('issues.total', { count: issuesStore.activeIssues?.length || 0 }) }}
            </UBadge>
        </div>
        
        <div v-if="issuesStore.isLoadingActive" class="space-y-3">
            <UCard v-for="i in 3" :key="i" class="w-full">
                <div class="flex items-center justify-between">
                    <div class="space-y-2 w-2/3">
                        <USkeleton class="h-4 w-[120px]" />
                        <USkeleton class="h-5 w-full" />
                    </div>
                    <USkeleton class="h-6 w-20 rounded-full" />
                </div>
            </UCard>
        </div>
        
        <div v-else-if="!issuesStore.activeIssues || issuesStore.activeIssues.length === 0"
            class="flex flex-col items-center justify-center p-12 text-center border-2 border-dashed border-neutral-200 dark:border-neutral-800 rounded-xl">
            <UIcon name="i-heroicons-clipboard-document-check" class="w-12 h-12 text-neutral-400 mb-3" />
            <h3 class="text-base font-semibold text-neutral-900 dark:text-white">
                {{ $t('issues.emptyTitle') }}
            </h3>
            <p class="text-sm text-neutral-500 dark:text-neutral-400 mt-1 max-w-xs">
                {{ $t('issues.emptyDesc') }}
            </p>
        </div>
        
        <div v-else class="space-y-3">
            <div v-for="issue in issuesStore.activeIssues" :key="issue.id"
                @click="handleIssueClick(issue.projectId, issue.id)"
                class="group block p-4 bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 hover:border-primary-500 dark:hover:border-primary-400 rounded-xl shadow-sm transition-all cursor-pointer duration-200 hover:shadow-md">
                <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div class="space-y-1.5 flex-1">
                        <div class="flex items-center gap-2 text-xs font-medium text-neutral-500 dark:text-neutral-400">
                            <span class="flex items-center gap-1">
                                <UIcon :name="getTypeConfig(issue.type).icon" class="w-4 h-4" />
                                <span class="font-semibold group-hover:text-primary-500 transition-colors">#{{ issue.number }}</span>
                            </span>
                            <span>•</span>
                            <span class="flex items-center gap-1">
                                <UIcon :name="getPriorityConfig(issue.priority).icon" class="w-3.5 h-3.5" />
                                {{ getPriorityConfig(issue.priority).label }}
                            </span>
                            <template v-if="issue.storyPoints">
                                <span>•</span>
                                <span class="bg-neutral-100 dark:bg-neutral-800 px-1.5 py-0.5 rounded text-neutral-600 dark:text-neutral-300">
                                    {{ issue.storyPoints }} SP
                                </span>
                            </template>
                        </div>
                        <h2 class="text-base font-semibold text-neutral-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors line-clamp-1">
                            {{ issue.title }}
                        </h2>
                    </div>
                    <div class="flex sm:flex-col items-start sm:items-end justify-between sm:justify-center gap-2 shrink-0">
                        <UBadge :color="getStatusConfig(issue.status).color" variant="subtle" size="md" class="capitalize font-medium">
                            <UIcon :name="getStatusConfig(issue.status).icon" class="w-3.5 h-3.5 mr-1" />
                            {{ getStatusConfig(issue.status).label }}
                        </UBadge>
                        <span class="text-xs text-neutral-400 dark:text-neutral-500 flex items-center gap-1">
                            <UIcon name="i-heroicons-clock" class="w-3.5 h-3.5" />
                            {{ $t('issues.updatedAt', { date: formatDate(issue.updatedAt) }) }}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>