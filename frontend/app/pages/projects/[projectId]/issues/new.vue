<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { IssuePriority, IssueType } from '~/types/issue'
import type { ProjectMemberDto } from '~/types/projects'

const { t } = useI18n()
const route = useRoute()
const projectId = computed(() => route.params.projectId as string)

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()

const { isSubmitting } = storeToRefs(issuesStore)
const { selectedProject } = storeToRefs(projectsStore)

const members = ref<ProjectMemberDto[]>([])
const isLoadingMembers = ref(false)

const tagInput = ref('')

const form = reactive({
  title: '',
  description: '',
  type: 3 as IssueType,
  priority: 2 as IssuePriority,
  storyPoints: undefined as number | undefined,
  assigneeId: undefined as string | undefined
})

const typeOptions = computed(() => [
  { label: t('issues.types.bug'), value: 0, icon: 'i-lucide-bug', color: 'text-rose-500 bg-rose-50 dark:bg-rose-950/30' },
  { label: t('issues.types.feature'), value: 1, icon: 'i-lucide-sparkles', color: 'text-primary-500 bg-primary-50 dark:bg-primary-950/30' },
  { label: t('issues.types.improvement'), value: 2, icon: 'i-lucide-trending-up', color: 'text-amber-500 bg-amber-50 dark:bg-amber-950/30' },
  { label: t('issues.types.task'), value: 3, icon: 'i-lucide-check-square', color: 'text-neutral-500 bg-neutral-50 dark:bg-neutral-900' }
])

const priorityOptions = computed(() => [
  { label: t('issues.priorities.low'), value: 0, icon: 'i-lucide-arrow-down', color: 'text-neutral-400' },
  { label: t('issues.priorities.medium'), value: 1, icon: 'i-lucide-minus', color: 'text-info-500' },
  { label: t('issues.priorities.high'), value: 2, icon: 'i-lucide-arrow-up', color: 'text-warning-500' },
  { label: t('issues.priorities.critical'), value: 3, icon: 'i-lucide-alert-triangle', color: 'text-error-500 animate-pulse' }
])

const fetchProjectMembers = async () => {
  if (!projectId.value) return
  isLoadingMembers.value = true
  try {
    members.value = await projectsStore.getMembers(projectId.value)
  } catch (error) {
    console.error('Не удалось загрузить участников:', error)
  } finally {
    isLoadingMembers.value = false
  }
}
const preparedTags = computed(() => {
  if (!tagInput.value.trim()) return []
  return [...new Set(tagInput.value.split(',').map(t => t.trim()).filter(Boolean))]
})

onMounted(() => {
  fetchProjectMembers()
  if (!selectedProject.value) {
    projectsStore.fetchProjects()
  }
})
const assigneeOptions = computed(() => {
  return members.value.map((m) => ({
    label: `${m.firstName} ${m.lastname}`.trim() || m.email,
    value: m.userId
  }))
})

const handleSubmit = async () => {
  const result = await issuesStore.create(
    projectId.value,
    {
    title: form.title,
    description: form.description || null,
    type: form.type,
    priority: form.priority,
    storyPoints: form.storyPoints ?? null,
    assigneeId: form.assigneeId,
    tags: preparedTags.value
  })

  if (result) {
    await navigateTo(`/projects/${projectId.value}/issues`)
  }
}
</script>

<template>
  <div class="space-y-6 max-w-3xl mx-auto px-4 py-8">
    <div class="flex items-center gap-3.5">
      <UButton :to="`/projects/${projectId}/issues`" icon="i-lucide-arrow-left" color="neutral" variant="ghost" class="rounded-xl transition-all hover:bg-neutral-100 dark:hover:bg-neutral-800" />
      <div>
        <h1 class="text-2xl font-black tracking-tight text-neutral-900 dark:text-white">
          {{ $t('issues.newIssueTitle') }}
        </h1>
        <p class="text-neutral-500 dark:text-neutral-400 text-sm mt-0.5">
          {{ $t('issues.newIssueSubtitle') }}
        </p>
      </div>
    </div>

    <UCard class="rounded-2xl shadow-sm border border-neutral-200/80 dark:border-neutral-800/80">
      <form class="space-y-6" @submit.prevent="handleSubmit">
        
        <UFormField :label="$t('issues.formTitleLabel')" required>
          <UInput v-model="form.title" :placeholder="$t('issues.formTitlePlaceholder')" class="w-full" size="lg" required />
        </UFormField>

        <UFormField :label="$t('issues.formDescLabel')">
          <UTextarea v-model="form.description" :placeholder="$t('issues.formDescPlaceholder')" :rows="5" class="w-full text-base" />
        </UFormField>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-5 p-4.5 rounded-xl bg-neutral-50 dark:bg-neutral-900/40 border border-neutral-100 dark:border-neutral-800/50">
          
          <UFormField :label="$t('issues.formTypeLabel')">
            <USelectMenu v-model="form.type" :items="typeOptions" value-key="value" class="w-full">
              <template #leading>
                <span class="flex items-center gap-2">
                  <UIcon :name="typeOptions.find(o => o.value === form.type)?.icon" :class="typeOptions.find(o => o.value === form.type)?.color.split(' ')[0]" />
                </span>
              </template>
            </USelectMenu>
          </UFormField>

          <UFormField :label="$t('issues.formPriorityLabel')">
            <USelectMenu v-model="form.priority" :items="priorityOptions" value-key="value" class="w-full">
              <template #leading>
                <UIcon :name="priorityOptions.find(o => o.value === form.priority)?.icon" :class="priorityOptions.find(o => o.value === form.priority)?.color" />
              </template>
            </USelectMenu>
          </UFormField>

          <UFormField :label="$t('issues.fieldAssignee')">
            <USelectMenu v-model="form.assigneeId" :items="assigneeOptions" value-key="value" :placeholder="$t('issues.formAssigneePlaceholder')" class="w-full" clearable>
              <template #leading>
                <UIcon name="i-lucide-user-check" class="text-neutral-400" />
              </template>
            </USelectMenu>
          </UFormField>

          <UFormField label="Story Points">
            <UInput v-model.number="form.storyPoints" type="number" min="0" placeholder="Например: 5" class="w-full">
              <template #leading>
                <UIcon name="i-lucide-component" class="text-neutral-400" />
              </template>
            </UInput>
          </UFormField>
        </div>

        <UFormField :label="$t('issues.fieldTags')" :hint="$t('issues.formTagsHint')">
          <div class="space-y-2.5">
            <UInput v-model="tagInput" :placeholder="$t('issues.formTagsPlaceholder')" class="w-full">
              <template #leading>
                <UIcon name="i-lucide-tags" class="text-neutral-400" />
              </template>
            </UInput>
            
            <div v-if="preparedTags.length" class="flex flex-wrap gap-1.5 pt-0.5">
              <span v-for="tag in preparedTags" :key="tag" class="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-md text-xs font-semibold bg-primary-50 text-primary-700 dark:bg-primary-950/40 dark:text-primary-300 border border-primary-200/60 dark:border-primary-800/50">
                <UIcon name="i-lucide-hash" class="w-3 h-3 text-primary-400 dark:text-primary-500" />
                {{ tag }}
              </span>
            </div>
          </div>
        </UFormField>

        <div class="flex justify-end gap-3 pt-4 border-t border-neutral-100 dark:border-neutral-800">
          <UButton :to="`/projects/${projectId}/issues`" color="neutral" variant="ghost" class="rounded-xl px-5 font-semibold">
            {{ $t('issues.cancelBtn') }}
          </UButton>
          <UButton type="submit" color="primary" class="rounded-xl px-6 font-bold shadow-sm" :loading="isSubmitting">
            {{ $t('issues.createBtn') }}
          </UButton>
        </div>
      </form>
    </UCard>
  </div>
</template>