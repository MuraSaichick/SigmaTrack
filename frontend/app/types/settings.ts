export type ContactVisibility = 'Everyone' | 'TeamOnly' | 'Nobody'
export type BirthDateVisibility = 'FullDate' | 'MonthAndDayOnly' | 'Hidden'
export type InvitationRestriction = 'Everyone' | 'TeamOnly'

export interface PrivacySettings {
  showContacts: ContactVisibility
  showBirthDate: BirthDateVisibility
  showOnlineStatus: boolean
  whoCanInviteMe: InvitationRestriction
  searchable: boolean
  showStatusMessage: boolean
}

export interface UpdateEmailRequest {
  newEmail: string
}

export interface UpdatePasswordRequest {
  currentPassword?: string
  newPassword?: string
  confirmPassword?: string
}