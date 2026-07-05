export interface NotificationDto {
  id: number
  type: NotificationType
  title: string
  message: string
  isRead: boolean
  linkUrl?: string
  createdAt: string
}

export enum NotificationType {
  IssueAssigned = 1,
  IssueStatusChanged = 2,
  NewComment = 3,
  UserMentioned = 4,
  ProjectInvitationReceived = 10,
  ProjectInvitationAccepted = 11,
  ProjectInvitationRejected = 12,
  ProjectMemberRemoved = 13,
  ProjectRoleChanged = 14,
  SprintStarted = 20,
  SprintCompleted = 21,
  SystemAlert = 90,
  SystemMaintenance = 91
}