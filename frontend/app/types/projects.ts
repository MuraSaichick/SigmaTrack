export interface UserProjectDto {
  projectId: string
  name: string
  description?: string
  prefix: string
  creatorId: string
  createdAt: string
  projectRoleId: number
}
export interface UpdateProjectDetailsRequest {
  name: string
  prefix: string
  description?: string
}
export interface SelectMenuProjectItem extends UserProjectDto {
  label: string
  id: string
  currentUserRole: number
}

export interface CreateProjectRequest {
  name: string
  description?: string
  prefix: string
}

export interface CreateProjectResponse {
  projectId: string
  name: string
  description?: string
  prefix: string
  creatorId: string
  createdAt: string
}

export interface ProjectMemberDto {
  memberId: string
  userId: string
  firstName: string
  lastname: string
  patronymic: string | null
  email: string
  projectRoleId: number
  projectRoleName: string
  joinedAt: string
}
export interface UserSearchResultDto {
  id: string
  fullName: string
  email: string
  avatarUrl?: string | null
  position?: string | null  
}

export interface CreateInvitationRequest {
  inviteeEmail: string
  projectRoleId: number
}

export interface CreateInvitationResponse {
  invitationId: string
  status: string
}