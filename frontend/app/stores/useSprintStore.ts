import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { SprintLookupDto, CreateSprintDto, SprintDetailsResponse, AddIssuesToSprintResponse } from '~/types/sprint'
import { SprintStatus} from '~/types/sprint'

export const useSprintStore = defineStore('sprint', () => {
  const api = useApi()
  const toast = useToast()

  const sprints = ref<SprintLookupDto[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  
  const selectedStatus = ref<SprintStatus | null>(null)

  const isLoadingDetail = ref(false)
  const isSubmittingAction = ref(false)
  const currentSprint = ref<SprintDetailsResponse | null>(null)

  async function fetchProjectSprints(projectId: string, status?: SprintStatus | 'All' | null) {
    isLoading.value = true
    error.value = null

    try {
      const queryParams: Record<string, string> = {}
      if (status) {
        queryParams.status = status
      }

      const data = await api<SprintLookupDto[]>(`/api/projects/${projectId}/sprints`, {
        query: queryParams
      })

      sprints.value = data
    } catch (err: any) {
      error.value = err.data?.message || 'Не удалось загрузить список спринтов'
      sprints.value = []
    } finally {
      isLoading.value = false
    }
  }
  
async function createSprint(projectId: string, sprintData: CreateSprintDto): Promise<boolean> {
  isLoading.value = true
  error.value = null
  
  try {
    await api(`/api/projects/${projectId}/sprints`, {
      method: 'POST',
      body: sprintData 
    })
    return true
  } catch (err: any) {
    console.error('Ошибка при создании спринта:', err)
    error.value = err.data?.message || 'Не удалось создать спринт'
    return false
  } finally {
    isLoading.value = false
  }
}

async function fetchSprintDetails(projectId: string, sprintId: string) {
    isLoadingDetail.value = true
    error.value = null
    try {
      currentSprint.value = await api<SprintDetailsResponse>(`/api/projects/${projectId}/sprints/${sprintId}`)
    } catch (err: any) {
      error.value = err.data?.message || 'Не удалось загрузить детальную информацию спринта'
      currentSprint.value = null
    } finally {
      isLoadingDetail.value = false
    }
  }
  async function startSprint(projectId: string, sprintId: string): Promise<boolean> {
    isSubmittingAction.value = true
    try {
      await api(`/api/projects/${projectId}/sprints/${sprintId}/start`, { method: 'PUT' })
      toast.add({ title: 'Успех', description: 'Спринт успешно запущен', color: 'success' })
      await fetchSprintDetails(projectId, sprintId)
      return true
    } catch (err: any) {
      toast.add({ title: 'Ошибка', description: err.data?.message || 'Не удалось запустить спринт', color: 'error' })
      return false
    } finally {
      isSubmittingAction.value = false
    }
  }
  async function completeSprint(projectId: string, sprintId: string): Promise<boolean> {
    isSubmittingAction.value = true
    try {
      await api(`/api/projects/${projectId}/sprints/${sprintId}/complete`, { method: 'PUT' })
      toast.add({ title: 'Успех', description: 'Спринт успешно завершен', color: 'success' })
      await fetchSprintDetails(projectId, sprintId)
      return true
    } catch (err: any) {
      toast.add({ title: 'Ошибка', description: err.data?.message || 'Не удалось завершить спринт', color: 'error' })
      return false
    } finally {
      isSubmittingAction.value = false
    }
  }
  async function addIssuesToSprint(projectId: string, sprintId: string, issueIds: string[]): Promise<boolean> {
    isSubmittingAction.value = true
    try {
      await api<AddIssuesToSprintResponse>(`/api/projects/${projectId}/sprints/${sprintId}/issues`, {
        method: 'POST',
        body: { issueIds }
      })
      toast.add({ title: 'Успех', description: 'Задачи успешно добавлены в спринт', color: 'success' })
      await fetchSprintDetails(projectId, sprintId)
      return true
    } catch (err: any) {
      toast.add({ title: 'Ошибка', description: err.data?.message || 'Не удалось добавить задачи', color: 'error' })
      return false
    } finally {
      isSubmittingAction.value = false
    }
  }
  async function cancelSprint(projectId: string, sprintId: string): Promise<boolean> {
    isSubmittingAction.value = true
    try {
      await api(`/api/projects/${projectId}/sprints/${sprintId}/cancel`, { method: 'PUT' })
      toast.add({ title: 'Предупреждение', description: 'Спринт отменен. Активные задачи возвращены в бэклог', color: 'warning' })
      await fetchSprintDetails(projectId, sprintId)
      return true
    } catch (err: any) {
      toast.add({ title: 'Ошибка', description: err.data?.message || 'Не удалось отменить спринт', color: 'error' })
      return false
    } finally {
      isSubmittingAction.value = false
    }
  }
  async function removeIssueFromSprint(projectId: string, sprintId: string, issueId: string): Promise<boolean> {
    isSubmittingAction.value = true
    try {
      await api(`/api/projects/${projectId}/sprints/${sprintId}/issues/${issueId}`, { method: 'DELETE' })
      toast.add({ title: 'Успех', description: 'Задача возвращена в бэклог', color: 'success' })
      await fetchSprintDetails(projectId, sprintId)
      return true
    } catch (err: any) {
      toast.add({ title: 'Ошибка', description: err.data?.message || 'Не удалось удалить задачу из спринта', color: 'error' })
      return false
    } finally {
      isSubmittingAction.value = false
    }
  }

  function $reset() {
    sprints.value = []
    selectedStatus.value = null
    error.value = null
    isLoading.value = false
  }

  return {
    sprints,
    isLoading,
    error,
    selectedStatus,
    currentSprint,
    isSubmittingAction,
    isLoadingDetail,
    fetchProjectSprints,
    createSprint,
    fetchSprintDetails,
    startSprint,
    completeSprint,
    addIssuesToSprint,
    cancelSprint,
    removeIssueFromSprint,
    $reset
  }
})