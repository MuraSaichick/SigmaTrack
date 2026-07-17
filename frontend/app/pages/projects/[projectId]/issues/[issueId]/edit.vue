<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { IssueDetailResponse, IssuePriority, IssueSeverity } from '~/types/issue'

const { t } = useI18n()
const route = useRoute()
const projectId = computed(() => route.params.projectId as string)
const issueId = computed(() => route.params.issueId as string)

const issuesStore = useIssuesStore()
const { isLoadingDetail, isSubmitting } = storeToRefs(issuesStore)

const form = reactive({
  title: '',
  description: '',
  priority: 2 as IssuePriority,
  severity: undefined as IssueSeverity | undefined,
  dueDate: '',
  estimatedHours: undefined as number | undefined,
  remainingHours: undefined as number | undefined,
  component: '',
  version: '',
  fixVersion: '',
  environment: '',
  tags: [] as string[],
  isReproducible: false,
  stepsToReproduce: ''
})

const priorityOptions = computed(() => [
  { label: t('issues.priorities.critical'), value: 0 },
  { label: t('issues.priorities.high'), value: 1 },
  { label: t('issues.priorities.medium'), value: 2 },
  { label: t('issues.priorities.low'), value: 3 },
  { label: t('issues.priorities.trivial'), value: 4 }
])

const severityOptions = computed(() => [
  { label: t('issues.severityLevels.critical'), value: 0 },
  { label: t('issues.severityLevels.major'), value: 1 },
  { label: t('issues.severityLevels.minor'), value: 2 },
  { label: t('issues.severityLevels.trivial'), value: 3 },
  { label: t('issues.severityLevels.enhancement'), value: 4 }
])

const populateForm = (issue: IssueDetailResponse) => {
  form.title = issue.title
  form.description = issue.description ?? ''
  form.priority = issue.priority
  form.severity = issue.severity ?? undefined
  form.dueDate = issue.dueDate ? issue.dueDate.slice(0, 10) : ''
  form.estimatedHours = issue.estimatedHours ?? undefined
  form.remainingHours = issue.remainingHours ?? undefined
  form.component = issue.component ?? ''
  form.version = issue.version ?? ''
  form.fixVersion = issue.fixVersion ?? ''
  form.environment = issue.environment ?? ''
  form.tags = [...issue.tags]
  form.isReproducible = issue.isReproducible
  form.stepsToReproduce = issue.stepsToReproduce ?? ''
}

const loadIssue = async () => {
  await issuesStore.fetchIssueById(issueId.value)
  if (issuesStore.currentIssue) {
    populateForm(issuesStore.currentIssue)
  }
}

onMounted(loadIssue)
watch(issueId, loadIssue)

const handleSubmit = async () => {
  const success = await issuesStore.updateDetails(issueId.value, {
    title: form.title,
    description: form.description || null,
    priority: form.priority,
    severity: form.severity ?? null,
    dueDate: form.dueDate ? new Date(form.dueDate).toISOString() : null,
    estimatedHours: form.estimatedHours ?? null,
    remainingHours: form.remainingHours ?? null,
    component: form.component || null,
    version: form.version || null,
    fixVersion: form.fixVersion || null,
    environment: form.environment || null,
    tags: form.tags,
    isReproducible: form.isReproducible,
    stepsToReproduce: form.stepsToReproduce || null
  })

  if (success) {
    await navigateTo(`/projects/${projectId.value}/issues/${issueId.value}`)
  }
}
</script>
<template>
  <div class="space-y-6 max-w-3xl mx-auto">
    <div class="flex items-center gap-3">
      <UButton
        :to="`/projects/${projectId}/issues/${issueId}`"
        icon="i-lucide-arrow-left"
        color="neutral"
        variant="ghost"
        class="rounded-xl"
      />
      <div>
        <h1 class="text-2xl font-black tracking-tight text-neutral-900 dark:text-white">
          {{ $t('issues.editPage.title') }}
        </h1>
        <p class="text-neutral-500 dark:text-neutral-400 text-sm mt-0.5">
          {{ $t('issues.editPage.subtitle') }}
        </p>
      </div>
    </div>

    <div v-if="isLoadingDetail" class="space-y-4">
      <USkeleton class="h-12 w-full rounded-xl" />
      <USkeleton class="h-32 w-full rounded-xl" />
      <USkeleton class="h-12 w-full rounded-xl" />
    </div>

    <UCard v-else class="rounded-2xl">
      <form class="space-y-5" @submit.prevent="handleSubmit">
        <UFormField :label="$t('issues.formTitleLabel')" required>
          <UInput v-model="form.title" class="w-full" required />
        </UFormField>

        <UFormField :label="$t('issues.description')">
          <UTextarea v-model="form.description" :rows="5" class="w-full" />
        </UFormField>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <UFormField :label="$t('issues.fieldPriority')">
            <USelectMenu
              v-model="form.priority"
              :items="priorityOptions"
              value-key="value"
              class="w-full"
            />
          </UFormField>

          <UFormField :label="$t('issues.fieldSeverity')">
            <USelectMenu
              v-model="form.severity"
              :items="severityOptions"
              value-key="value"
              class="w-full"
            />
          </UFormField>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <UFormField :label="$t('issues.deadline')">
            <UInput v-model="form.dueDate" type="date" class="w-full" />
          </UFormField>
          <UFormField :label="$t('issues.timeEstimation')">
            <UInput v-model.number="form.estimatedHours" type="number" min="0" step="0.5" class="w-full" />
          </UFormField>
          <UFormField :label="$t('issues.timeRemaining')">
            <UInput v-model.number="form.remainingHours" type="number" min="0" step="0.5" class="w-full" />
          </UFormField>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <UFormField :label="$t('issues.fieldComponent')">
            <UInput v-model="form.component" class="w-full" />
          </UFormField>
          <UFormField :label="$t('issues.fieldEnvironment')">
            <UInput v-model="form.environment" class="w-full" />
          </UFormField>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <UFormField :label="$t('issues.fieldVersion')">
            <UInput v-model="form.version" class="w-full" />
          </UFormField>
          <UFormField :label="$t('issues.fieldFixVersion')">
            <UInput v-model="form.fixVersion" class="w-full" />
          </UFormField>
        </div>

        <UFormField :label="$t('issues.stepsToReproduce')">
          <UTextarea v-model="form.stepsToReproduce" :rows="3" class="w-full" />
        </UFormField>

        <UCheckbox v-model="form.isReproducible" :label="$t('issues.reproducibleStable')" />

        <div class="flex justify-end gap-2 pt-2 border-t border-neutral-100 dark:border-neutral-800">
          <UButton
            :to="`/projects/${projectId}/issues/${issueId}`"
            color="neutral"
            variant="ghost"
            class="rounded-xl font-bold"
          >
            {{ $t('issues.cancelBtn') }}
          </UButton>
          <UButton
            type="submit"
            color="primary"
            class="rounded-xl font-bold"
            :loading="isSubmitting"
          >
            {{ $t('issues.editPage.saveBtn') }}
          </UButton>
        </div>
      </form>
    </UCard>
  </div>
</template>