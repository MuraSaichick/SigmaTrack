export type IssueSeverity = 0 | 1 | 2 | 3 | 4

export interface IssueDto {
  id: string
  number: number
  title: string
  status: IssueStatus
  type: IssueType
  priority: IssuePriority
  storyPoints: number | null
  assigneeId: string | null
  updatedAt: string
}

export interface PagedListResponse<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
}

export interface GetIssuesListQuery {
  projectId: string
  status?: IssueStatus
  type?: IssueType
  priority?: IssuePriority
  assigneeId?: string
  searchTerm?: string
  pageNumber?: number
  pageSize?: number
}

export interface CommentDto {
  id: string
  authorId: string
  authorName: string
  text: string
  createdAt: string
  updatedAt: string | null
  mentions: string[]
  attachmentUrls: string[]
}

export interface IssueLinkDto {
  id: string
  targetIssueId: string
  targetIssueNumber: string
  targetIssueTitle: string
  linkType: string
  description: string | null
}

export interface IssueDetailResponse {
  id: string
  projectId: string
  number: number
  title: string
  description: string | null
  status: IssueStatus
  type: IssueType
  priority: IssuePriority
  severity: IssueSeverity | null
  reporterId: string
  reporterName: string
  assigneeId: string | null
  assigneeName: string | null
  createdAt: string
  updatedAt: string
  dueDate: string | null
  startedAt: string | null
  resolvedAt: string | null
  closedAt: string | null
  storyPoints: number | null
  estimatedHours: number | null
  loggedHours: number | null
  remainingHours: number | null
  component: string | null
  version: string | null
  fixVersion: string | null
  environment: string | null
  tags: string[]
  isReproducible: boolean
  stepsToReproduce: string | null
  isBlocked: boolean
  blockReason: string | null
  blockedByIssueId: string | null
  viewCount: number
  comments: CommentDto[]
  links: IssueLinkDto[]
}

export interface CreateIssueRequest {
  projectId: string
  title: string
  description?: string | null
  type: IssueType
  priority: IssuePriority
  assigneeId?: string | null
  storyPoints?: number | null
  tags: string[]
}

export interface CreateIssueResponse {
  id: string
  number: number
  title: string
  status: IssueStatus
  reporterId: string      
  assigneeId?: string | null
  tags: string[]           
}

export interface ChangeStatusRequest {
  newStatus: IssueStatus
}


export interface AssignIssueRequest {
  assigneeId: string | null
}


export interface UpdateIssueRequest {
  title: string
  description?: string | null
  priority: IssuePriority
  severity?: IssueSeverity | null
  dueDate?: string | null
  estimatedHours?: number | null
  remainingHours?: number | null
  component?: string | null
  version?: string | null
  fixVersion?: string | null
  environment?: string | null
  tags: string[]
  isReproducible: boolean
  stepsToReproduce?: string | null
}


export interface MessageResponse {
  message: string
}

export interface AttachmentInput {
  filename: string
  fileUrl: string
  fileSize: number
  contentType: string
}

export interface AddCommentRequest {
  text: string
  isInternal: boolean
  mentions?: string[]
  attachments?: AttachmentInput[]
}

export interface AttachmentResponseDto {
  id: string
  filename: string
  fileUrl: string
  fileSize: number
  contentType: string
  uploadedAt: string
}

export interface CommentResponseDto {
  id: string
  authorId: string
  authorName: string
  authorAvatarUrl: string | null
  authorAvatarColor: string | null
  text: string
  createdAt: string
  isInternal: boolean
  attachments: AttachmentResponseDto[]
}

export enum IssueStatus {
  Backlog = 0,
  ToDo = 1,
  InProgress = 2,
  InReview = 3,
  Testing = 4,
  Resolved = 5,
  Closed = 6,
  Reopened = 7,
  Rejected = 8,
  OnHold = 9
} 

export enum IssueType {
  Task = 1,
  Bug = 2,
  Story = 3,
  Epic = 4
}

export enum IssuePriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Critical = 4
}

export interface UserActiveIssueResponse {
  id: string
  projectId: string
  number: number
  title: string
  status: IssueStatus
  type: IssueType
  priority: IssuePriority
  storyPoints?: number
  assigneeId?: string
  updatedAt: string
}