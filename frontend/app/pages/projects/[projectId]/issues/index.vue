<script setup lang="ts">
import { storeToRefs } from 'pinia'

const route = useRoute()
const projectId = computed(() => route.params.projectId as string)

const issuesStore = useIssuesStore()
const { issuesList, isLoadingList, totalCount } = storeToRefs(issuesStore)

const loadIssues = () => issuesStore.fetchIssues(projectId.value)

onMounted(loadIssues)
watch(projectId, loadIssues)
</script>

<template>
  <div class="space-y-6 max-w-6xl mx-auto">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-neutral-200 dark:border-neutral-800 pb-5">
      <div>
        <h1 class="text-3xl font-black tracking-tight text-neutral-900 dark:text-white">
          Задачи
        </h1>
        <p class="text-neutral-500 dark:text-neutral-400 text-sm mt-1">
          {{ totalCount > 0 ? `${totalCount} задач в проекте` : 'Список задач проекта' }}
        </p>
      </div>

      <UButton
        :to="`/projects/${projectId}/issues/new`"
        icon="i-lucide-plus"
        color="primary"
        class="rounded-xl font-bold shrink-0"
      >
        Создать задачу
      </UButton>
    </div>

    <div v-if="isLoadingList" class="space-y-3">
      <USkeleton v-for="i in 5" :key="i" class="h-16 w-full rounded-xl" />
    </div>

    <div
      v-else-if="issuesList.length === 0"
      class="bg-neutral-50 dark:bg-neutral-900/40 border border-neutral-200 dark:border-neutral-800 rounded-2xl p-12 text-center"
    >
      <UIcon name="i-lucide-list-todo" class="w-10 h-10 text-neutral-300 dark:text-neutral-700 mx-auto mb-3" />
      <p class="text-sm font-medium text-neutral-500 dark:text-neutral-400">
        Задач пока нет
      </p>
      <p class="text-xs text-neutral-400 dark:text-neutral-500 mt-1 mb-4">
        Создайте первую задачу для этого проекта
      </p>
      <UButton
        :to="`/projects/${projectId}/issues/new`"
        icon="i-lucide-plus"
        variant="soft"
        class="rounded-xl font-bold"
      >
        Создать задачу
      </UButton>
    </div>

    <div v-else class="bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-2xl overflow-hidden shadow-xs">
      <div class="divide-y divide-neutral-100 dark:divide-neutral-800/50">
        <NuxtLink
          v-for="issue in issuesList"
          :key="issue.id"
          :to="`/projects/${projectId}/issues/${issue.id}`"
          class="flex items-center justify-between gap-4 p-4 hover:bg-neutral-50/60 dark:hover:bg-neutral-800/20 transition-colors"
        >
          <div class="flex items-center gap-3 min-w-0 flex-1">
            <UIcon
              :name="getTypeConfig(issue.type).icon"
              class="w-4 h-4 shrink-0"
              :class="`text-${getTypeConfig(issue.type).color}-500`"
            />
            <span class="text-xs font-mono text-neutral-400 shrink-0">#{{ issue.number }}</span>
            <span class="text-sm font-medium text-neutral-900 dark:text-white truncate">
              {{ issue.title }}
            </span>
          </div>
          <div class="flex items-center gap-3 shrink-0">
            <UBadge
              :color="getStatusConfig(issue.status).color"
              variant="subtle"
              size="sm"
            >
              {{ getStatusConfig(issue.status).label }}
            </UBadge>
            <UIcon name="i-lucide-chevron-right" class="w-4 h-4 text-neutral-400" />
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>
