import type {
  AssignIssueRequest,
  ChangeStatusRequest,
  CreateIssueRequest,
  CreateIssueResponse,
  GetIssuesListQuery,
  IssueDetailResponse,
  MessageResponse,
  PagedListResponse,
  IssueDto,
  IssueStatus,
  UpdateIssueRequest,
  AddCommentRequest,     
  CommentResponseDto,
  UserActiveIssueResponse
} from '~/types/issue'

export const useIssues = () => {
  const apiFetch = useApi()

  const getUserActiveIssues = async (userId: string): Promise<UserActiveIssueResponse[]> => {
    return await apiFetch<UserActiveIssueResponse[]>(`/api/issues/users/${userId}/active-issues`, {
      method: 'GET'
    })
  }

  const getIssues = (
    projectId: string,
    params: Omit<GetIssuesListQuery, 'projectId'> = {}
  ) =>
    apiFetch<PagedListResponse<IssueDto>>('/api/issues', {
      method: 'GET',
      query: { projectId, ...params }
    })

  const getIssueById = (id: string) =>
    apiFetch<IssueDetailResponse>(`/api/issues/${id}`, {
      method: 'GET'
    })

  const createIssue = (body: CreateIssueRequest) =>
    apiFetch<CreateIssueResponse>('/api/issues', {
      method: 'POST',
      body
    })

  const updateStatus = (id: string, status: IssueStatus) =>
    apiFetch<MessageResponse>(`/api/issues/${id}/status`, {
      method: 'PUT',
      body: { newStatus: status } satisfies ChangeStatusRequest
    })

  const updateAssignee = (id: string, userId: string | null) =>
    apiFetch<MessageResponse>(`/api/issues/${id}/assignee`, {
      method: 'PUT',
      body: { assigneeId: userId } satisfies AssignIssueRequest
    })

  const updateIssueDetails = (id: string, body: UpdateIssueRequest) =>
    apiFetch<MessageResponse>(`/api/issues/${id}`, {
      method: 'PUT',
      body
    })

    const addComment = (id: string, body: AddCommentRequest) =>
    apiFetch<CommentResponseDto>(`/api/issues/${id}/comments`, {
      method: 'POST',
      body
    })

  return {
    getIssues,
    getIssueById,
    createIssue,
    updateStatus,
    updateAssignee,
    updateIssueDetails,
    addComment,
    getUserActiveIssues
  }
}
