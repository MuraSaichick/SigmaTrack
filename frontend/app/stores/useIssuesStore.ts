import { ref } from 'vue'
import { defineStore } from 'pinia'
import type {
  AttachmentInput,
  CommentResponseDto,
  CreateIssueRequest,
  CreateIssueResponse,
  GetIssuesListQuery,
  IssueDetailResponse,
  IssueDto,
  IssueStatus,
  UpdateIssueRequest,
  UserActiveIssueResponse,
  ChangeStatusRequest,
  AssignIssueRequest,
  AddCommentRequest,
  MessageResponse,
  PagedListResponse
} from '~/types/issue'

export const useIssuesStore = defineStore('issues', () => {
  const apiFetch = useApi()
  const toast = useToast()

  const issuesList = ref<IssueDto[]>([])
  const totalCount = ref(0)
  const currentIssue = ref<IssueDetailResponse | null>(null)
  const activeIssues = ref<UserActiveIssueResponse[]>([])
  
  const isLoadingList = ref(false)
  const isLoadingDetail = ref(false)
  const isLoadingActive = ref(false)
  const isSubmitting = ref(false)
  const isAddingComment = ref(false)

  const fetchUserActiveIssues = async (userId: string) => {
    isLoadingActive.value = true
    try {
      activeIssues.value = await apiFetch<UserActiveIssueResponse[]>(`/api/issues/users/${userId}/active-issues`, {
        method: 'GET'
      })
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
    params?: GetIssuesListQuery
  ) => {
    isLoadingList.value = true
    try {
      const data = await apiFetch<PagedListResponse<IssueDto>>(`/api/projects/${projectId}/issues`, {
        method: 'GET',
        query: params
      })
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
      currentIssue.value = await apiFetch<IssueDetailResponse>(`/api/issues/${id}`, {
        method: 'GET'
      })
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
  const create = async (
    projectId: string, 
    body: CreateIssueRequest
  ): Promise<CreateIssueResponse | null> => {
    isSubmitting.value = true
    try {
      const payload = {
        ...body,
        tags: body.tags || []
      }

      const result = await apiFetch<CreateIssueResponse>(`/api/projects/${projectId}/issues`, {
        method: 'POST',
        body: payload
      })
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
      await apiFetch<MessageResponse>(`/api/issues/${id}/status`, {
        method: 'PUT',
        body: { newStatus: status } satisfies ChangeStatusRequest
      })

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
      await apiFetch<MessageResponse>(`/api/issues/${id}/assignee`, {
        method: 'PUT',
        body: { assigneeId } satisfies AssignIssueRequest
      })

      if (currentIssue.value?.id === id) {
        currentIssue.value = await apiFetch<IssueDetailResponse>(`/api/issues/${id}`, { method: 'GET' })
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
      await apiFetch<MessageResponse>(`/api/issues/${id}`, {
        method: 'PUT',
        body
      })
      currentIssue.value = await apiFetch<IssueDetailResponse>(`/api/issues/${id}`, { method: 'GET' })
      
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

  const postComment = async (
    id: string, 
    text: string, 
    isInternal: boolean = false, 
    attachments: AttachmentInput[] = []
  ): Promise<boolean> => {
    isAddingComment.value = true
    try {
      await apiFetch<CommentResponseDto>(`/api/issues/${id}/comments`, {
        method: 'POST',
        body: {
          text,
          isInternal,
          mentions: [],
          attachments
        } satisfies AddCommentRequest
      })

      if (currentIssue.value?.id === id) {
        currentIssue.value = await apiFetch<IssueDetailResponse>(`/api/issues/${id}`, { method: 'GET' })
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

  const clearCurrentIssue = () => {
    currentIssue.value = null
  }

  return {
    issuesList,
    totalCount,
    currentIssue,
    activeIssues,
    isLoadingList,
    isLoadingDetail,
    isLoadingActive,
    isSubmitting,
    isAddingComment,
    fetchIssues,
    fetchUserActiveIssues,
    fetchIssueById,
    create,
    changeStatus,
    assign,
    updateDetails,
    clearCurrentIssue,
    postComment
  }
})