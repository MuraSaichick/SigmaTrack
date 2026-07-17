export enum UserOnlineStatus {
  Online = 'Online',
  Away = 'Away',
  DoNotDisturb = 'DoNotDisturb',
  Offline = 'Offline'
}

export enum ContactVisibility {
  Everyone = 'Everyone',
  TeamOnly = 'TeamOnly',
  Nobody = 'Nobody'
}

export enum BirthDateVisibility {
  FullDate = 'FullDate',
  MonthAndDayOnly = 'MonthAndDayOnly',
  Nobody = 'Nobody'
}

export enum InvitationRestriction {
  Everyone = 'Everyone',
  ProjectMembersOnly = 'ProjectMembersOnly'
}

export interface PrivacySettings {
  showContacts: ContactVisibility;
  showBirthDate: BirthDateVisibility;
  showOnlineStatus: boolean;
  whoCanInviteMe: InvitationRestriction;
  searchable: boolean;
  showStatusMessage: boolean;
}

export interface UserProfileResponse {
  id: string; 
  login: string;
  firstname: string;
  lastname: string;
  patronymic: string | null; 
  avatarUrl: string | null;
  avatarColor: string | null;
  bio: string | null;
  position: string | null;
  department: string | null;
  skills: string[];
  gitHub: string | null;
  lastSeenAt: string | null; // ISO Date String
  onlineStatus: UserOnlineStatus;
  email: string | null;
  phoneNumber: string | null;
  telegram: string | null;
  birthDate: string | null; // ISO Date String или null
  showStatusMessage: boolean;
  statusMessage: string | null;
}

export interface UpdateProfileRequest {
  firstname: string;
  lastname: string;
  patronymic: string | null;
  phone: string;
  statusMessage: string | null;
  bio: string | null;
  position: string | null;
  department: string | null;
  skills: string[];
  birthDate: string | null;
  telegram: string | null;
  gitHub: string | null;
}

export interface UpdatePrivacyRequest {
  showContacts: ContactVisibility;
  showBirthDate: BirthDateVisibility;
  showOnlineStatus: boolean;
  whoCanInviteMe: InvitationRestriction;
  searchable: boolean;
  showStatusMessage: boolean;
}

export interface UserSearchDto {
  id: string;
  login: string;
  firstname: string;
  lastname: string;
  avatarUrl: string | null;
  avatarColor: string | null;
  position: string | null;
}

export interface SearchUsersResponse {
  users: UserSearchDto[];
  totalCount: number;
}