import { defineStore } from 'pinia'
import type {
  CreateIssueRequest,
  CreateIssueResponse,
  GetIssuesListQuery,
  IssueDetailResponse,
  IssueDto,
  IssueStatus,
  UpdateIssueRequest,
  UserActiveIssueResponse
} from '~/types/issue'

export const useIssuesStore = defineStore('issues', () => {
  const {
    getIssues,
    getIssueById,
    createIssue,
    updateStatus,
    updateAssignee,
    updateIssueDetails,
    addComment: apiAddComment,
    getUserActiveIssues
  } = useIssues()
  const toast = useToast()

  const issuesList = ref<IssueDto[]>([])
  const totalCount = ref(0)
  const currentIssue = ref<IssueDetailResponse | null>(null)
  const isLoadingList = ref(false)
  const isLoadingDetail = ref(false)
  const isSubmitting = ref(false)
  const isAddingComment = ref(false)

  const activeIssues = ref<UserActiveIssueResponse[]>([])
  const isLoadingActive = ref(false)

  const fetchUserActiveIssues = async (userId: string) => {
    isLoadingActive.value = true
    try {
      activeIssues.value = await getUserActiveIssues(userId)
    } catch (error: any) {
      activeIssues.value = []
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось загрузить ваши активные задачи',
        color: 'error'
      })
    } finally {
      isLoadingActive.value = false
    }
  }

  const fetchIssues = async (
    projectId: string,
    params: Omit<GetIssuesListQuery, 'projectId'> = {}
  ) => {
    isLoadingList.value = true
    try {
      const data = await getIssues(projectId, params)
      issuesList.value = data.items
      totalCount.value = data.totalCount
    } catch (error: any) {
      issuesList.value = []
      totalCount.value = 0
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось загрузить список задач',
        color: 'error'
      })
    } finally {
      isLoadingList.value = false
    }
  }

  const fetchIssueById = async (id: string) => {
    isLoadingDetail.value = true
    try {
      currentIssue.value = await getIssueById(id)
    } catch (error: any) {
      currentIssue.value = null
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось загрузить задачу',
        color: 'error'
      })
    } finally {
      isLoadingDetail.value = false
    }
  }

const create = async (body: CreateIssueRequest): Promise<CreateIssueResponse | null> => {
  isSubmitting.value = true
  try {
    const payload: CreateIssueRequest = {
      ...body,
      tags: body.tags || []
    }

    const result = await createIssue(payload)
    
    toast.add({
      title: 'Успех',
      description: `Задача #${result.number} создана`,
      color: 'success'
    })
    return result
  } catch (error: any) {
    toast.add({
      title: 'Ошибка',
      description: error.data?.message || 'Не удалось создать задачу',
      color: 'error'
    })
    return null
  } finally {
    isSubmitting.value = false
  }
}
  const changeStatus = async (id: string, status: IssueStatus): Promise<boolean> => {
    try {
      await updateStatus(id, status)
      if (currentIssue.value?.id === id) {
        currentIssue.value = { ...currentIssue.value, status }
      }
      toast.add({
        title: 'Успех',
        description: 'Статус задачи обновлён',
        color: 'success'
      })
      return true
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось изменить статус',
        color: 'error'
      })
      return false
    }
  }

  const assign = async (id: string, assigneeId: string | null): Promise<boolean> => {
    try {
      await updateAssignee(id, assigneeId)
      if (currentIssue.value?.id === id) {
        currentIssue.value = await getIssueById(id)
      }
      toast.add({
        title: 'Успех',
        description: 'Исполнитель задачи обновлён',
        color: 'success'
      })
      return true
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.error || error.data?.message || 'Не удалось назначить исполнителя',
        color: 'error'
      })
      return false
    }
  }

  const updateDetails = async (id: string, body: UpdateIssueRequest): Promise<boolean> => {
    isSubmitting.value = true
    try {
      await updateIssueDetails(id, body)
      await fetchIssueById(id)
      toast.add({
        title: 'Успех',
        description: 'Данные задачи обновлены',
        color: 'success'
      })
      return true
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось обновить задачу',
        color: 'error'
      })
      return false
    } finally {
      isSubmitting.value = false
    }
  }

  const clearCurrentIssue = () => {
    currentIssue.value = null
  }

  const postComment = async (id: string, text: string, isInternal: boolean = false): Promise<boolean> => {
    isAddingComment.value = true
    try {
      await apiAddComment(id, {
        text,
        isInternal,
        mentions: [],
        attachments: []
      })
      if (currentIssue.value?.id === id) {
        currentIssue.value = await getIssueById(id)
      }
      toast.add({
        title: 'Успех',
        description: 'Комментарий добавлен успешно',
        color: 'success'
      })
      return true
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось отправить комментарий',
        color: 'error'
      })
      return false
    } finally {
      isAddingComment.value = false
    }
  }
  return {
    issuesList,
    totalCount,
    currentIssue,
    isLoadingList,
    isLoadingDetail,
    isSubmitting,
    isAddingComment,
    fetchIssues,
    fetchIssueById,
    create,
    changeStatus,
    assign,
    updateDetails,
    clearCurrentIssue,
    postComment,
    activeIssues,
    isLoadingActive,
    fetchUserActiveIssues
  }
})