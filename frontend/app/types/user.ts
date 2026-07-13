export interface UserProfileResponse {
  id: string; 
  login: string;
  firstname: string;
  lastname: string;
  patronymic: string | null; 
  email: string;
  phone: string;
  avatarUrl: string | null;
  avatarColor: string | null;
  statusMessage: string | null;
  bio: string | null;
  position: string | null;
  department: string | null;
  skills: string[];
  birthDate: string | null; 
  telegram: string | null;
  gitHub: string | null;
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