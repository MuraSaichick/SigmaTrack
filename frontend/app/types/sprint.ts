export enum SprintStatus {
  Planning = 'Planning',
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export interface SprintLookupDto {
  id: string
  name: string
  goal?: string | null
  status: SprintStatus
  startDate: string
  endDate: string
  capacity: number
  committedPoints: number
}

export interface CreateSprintDto {
  name: string
  goal?: string | null
  startDate: string
  endDate: string
  capacity: number
}
export interface SprintIssueDto {
  id: string
  key: string
  title: string
  status: string
  type: string
  priority: string
  storyPoints: number | null
  assigneeName: string | null
}

export interface SprintDetailsResponse {
  id: string
  name: string
  goal?: string | null
  status: SprintStatus
  startDate: string
  endDate: string
  capacity: number
  committedPoints: number
  completedPoints: number
  issues: SprintIssueDto[]
}

export interface AddIssuesApiRequest {
  issueIds: string[]
}

export interface AddIssuesToSprintResponse {
  sprintId: string
  totalIssuesAdded: number
  newCommittedPoints: number
}