<script setup lang="ts">
import { ref, watch, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSprintStore } from '~/stores/useSprintStore'
import { SprintStatus } from '~/types/sprint'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()
const sprintStore = useSprintStore()

const projectId = computed(() => route.params.projectId as string)

interface SprintTab {
  key: string
  label: string
}

const tabs = computed<SprintTab[]>(() => [
  { key: 'All', label: t('sprints.statuses.all') },
  { key: SprintStatus.Planning, label: t('sprints.statuses.planning') },
  { key: SprintStatus.Active, label: t('sprints.statuses.active') },
  { key: SprintStatus.Completed, label: t('sprints.statuses.completed') },
  { key: SprintStatus.Cancelled, label: t('sprints.statuses.cancelled') }
])

const activeTabIndex = computed({
  get: () => {
    const currentStatus = (route.query.status as string) || 'All'
    const index = tabs.value.findIndex(tab => tab.key === currentStatus)
    return index !== -1 ? index : 0
  },
  set: (index) => {
    const selectedTab = tabs.value[index]
    if (!selectedTab) return

    router.push({
      query: { ...route.query, status: selectedTab.key === 'All' ? undefined : selectedTab.key }
    })
  }
})

const currentStatus = computed(() => tabs.value[activeTabIndex.value]?.key as SprintStatus | 'All')

const loadSprints = () => {
  if (!projectId.value || projectId.value === 'undefined') return
  const statusFilter = currentStatus.value === 'All' ? null : currentStatus.value
  sprintStore.fetchProjectSprints(projectId.value, statusFilter)
}

watch(activeTabIndex, () => {
  loadSprints()
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString(locale.value, {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  })
}

const getSprintStatusLabel = (status: SprintStatus): string => {
  const labels: Record<SprintStatus, string> = {
    [SprintStatus.Planning]: t('sprints.statuses.planning'),
    [SprintStatus.Active]: t('sprints.statuses.active'),
    [SprintStatus.Completed]: t('sprints.statuses.completed'),
    [SprintStatus.Cancelled]: t('sprints.statuses.cancelled')
  }
  return labels[status] || status
}

const getStatusColor = (status: SprintStatus): 'neutral' | 'success' | 'primary' | 'error' => {
  const colors: Record<SprintStatus, 'neutral' | 'success' | 'primary' | 'error'> = {
    [SprintStatus.Planning]: 'neutral',
    [SprintStatus.Active]: 'success',
    [SprintStatus.Completed]: 'primary',
    [SprintStatus.Cancelled]: 'error'
  }
  return colors[status] || 'neutral'
}

onMounted(() => {
  loadSprints()
})
</script>

<template>
  <div class="p-6 max-w-7xl mx-auto space-y-6">
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sprints.title') }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sprints.subtitle') }}
        </p>
      </div>

      <UButton icon="i-heroicons-plus" color="primary" variant="solid" :to="`/projects/${projectId}/sprints/create`">
        {{ $t('sprints.createBtn') }}
      </UButton>
    </div>

    <UTabs v-model="activeTabIndex" :items="tabs" class="w-full" :ui="{ root: 'space-y-4', list: 'max-w-2xl' }" />

    <div v-if="sprintStore.isLoading" class="flex justify-center py-12">
      <UIcon name="i-heroicons-arrow-path" class="animate-spin h-8 w-8 text-primary" />
    </div>

    <div v-else-if="sprintStore.error" class="max-w-4xl mx-auto">
      <UAlert icon="i-heroicons-exclamation-triangle" color="error" variant="soft" :title="$t('sprints.errorTitle')"
        :description="sprintStore.error" />
    </div>

    <div v-else-if="sprintStore.sprints.length === 0"
      class="text-center py-12 border border-dashed border-gray-300 dark:border-gray-800 rounded-xl">
      <UIcon name="i-heroicons-folder-open" class="h-10 w-10 text-gray-400 mx-auto mb-3" />
      <p class="text-gray-500 dark:text-gray-400">
        {{ $t('sprints.emptyTitle') }}
      </p>
    </div>

    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <UCard v-for="sprint in sprintStore.sprints" :key="sprint.id" as="NuxtLink"
        :to="`/projects/${projectId}/sprints/${sprint.id}`" class="flex flex-col justify-between" :ui="{ body: 'p-5' }">
        <div class="flex-1">
          <div class="flex items-start justify-between gap-2 mb-2">
            <NuxtLink :to="`/projects/${projectId}/sprints/${sprint.id}`"
              class="font-semibold text-gray-900 dark:text-white hover:text-primary transition-colors line-clamp-1">
              {{ sprint.name }}
            </NuxtLink>

            <UBadge :color="getStatusColor(sprint.status)" variant="subtle" size="sm">
              {{ getSprintStatusLabel(sprint.status) }}
            </UBadge>
          </div>

          <p class="text-sm text-gray-500 dark:text-gray-400 line-clamp-2 mb-4 h-10">
            {{ sprint.goal || $t('sprints.noGoal') }}
          </p>
        </div>

        <template #footer>
          <div class="space-y-3">
            <div class="flex justify-between text-xs text-gray-500 dark:text-gray-400">
              <span>
                {{ $t('sprints.committedPoints', { points: sprint.committedPoints }) }}
              </span>
              <span>
                {{ $t('sprints.capacityLimit', { limit: sprint.capacity }) }}
              </span>
            </div>

            <UProgress :model-value="sprint.committedPoints" :max="sprint.capacity" color="primary" size="sm" />

            <div class="flex items-center text-xs text-gray-400 dark:text-gray-500 gap-1 pt-1">
              <UIcon name="i-heroicons-calendar" class="h-3.5 w-3.5" />
              <span>{{ formatDate(sprint.startDate) }}</span>
              <span>—</span>
              <span>{{ formatDate(sprint.endDate) }}</span>
            </div>
          </div>
        </template>
      </UCard>
    </div>
  </div>
</template>