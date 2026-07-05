import { defineStore } from 'pinia'
import type { 
  UserProjectDto, 
  SelectMenuProjectItem, 
  CreateProjectRequest, 
  CreateProjectResponse, 
  ProjectMemberDto, 
  UpdateProjectDetailsRequest,
  UserSearchResultDto, 
  CreateInvitationRequest,
  CreateInvitationResponse
} from "~/types/projects"

export const useProjectsStore = defineStore('projects', () => {
  const api = useApi()
  const toast = useToast()
  const route = useRoute()
  const { user: currentUser } = useAuth()
  const projects = ref<UserProjectDto[]>([])
  const selectedProject = ref<SelectMenuProjectItem | undefined>(undefined)
  const projectItems = computed<SelectMenuProjectItem[]>(() => {
    return projects.value.map(p => ({
      ...p,
      label: p.name,
      id: p.projectId,
      currentUserRole: p.projectRoleId
    }))
  })

  const inviteUser = async (projectId: string, body: CreateInvitationRequest): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}/invitations`, {
        method: 'POST',
        body
      })
      toast.add({
        title: 'Успех',
        description: `Приглашение для ${body.inviteeEmail} успешно отправлено!`,
        color: 'success'
      })
      return true
    } catch (error: any) {
      toast.add({
        title: 'Ошибка',
        description: error.data?.message || 'Не удалось отправить приглашение',
        color: 'error'
      })
      return false
    }
  }

  const fetchProjects = async () => {
    try {
      const data = await api<UserProjectDto[]>('/api/projects/myProjects')
      projects.value = data
      const projectIdFromUrl = route.params.projectId as string
      
      if (projectIdFromUrl) {
        const currentProject = projectItems.value.find(p => p.id === projectIdFromUrl)
        if (currentProject) {
          selectedProject.value = currentProject
          return
        }
      }
      if (projectItems.value.length > 0) {
        selectedProject.value = projectItems.value[0] ?? undefined
      } else {
        selectedProject.value = undefined
      }
    } catch (error) {
      console.error('Ошибка при загрузке проектов:', error)
      clearStore()
    }
  }

  const createProject = async (projectData: CreateProjectRequest) => {
    try {
      const response = await api<CreateProjectResponse>('/api/projects', {
        method: 'POST',
        body: projectData
      })
      await fetchProjects()
      return response
    } catch (error) {
      console.error('Ошибка при создании проекта:', error)
      throw error
    }
  }

  const canEditTasks = computed(() => {
    if (!selectedProject.value) return false
    return [1, 2, 3].includes(selectedProject.value.currentUserRole)
  })

  const canManageProject = computed(() => {
    if (!selectedProject.value) return false
    return selectedProject.value.currentUserRole === 1
  })

  const isObserver = computed(() => {
    return selectedProject.value?.currentUserRole === 4
  })

  const getMembers = async (projectId: string): Promise<ProjectMemberDto[]> => {
    try {
      return await api<ProjectMemberDto[]>(`/api/projects/${projectId}/members`, { method: 'GET' })
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: 'Не удалось загрузить список участников', color: 'error' })
      return []
    }
  }

  const leaveProject = async (projectId: string): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}/leave`, { method: 'DELETE' })
      toast.add({ title: 'Успех', description: 'Вы успешно покинули проект', color: 'success' })
      return true
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: 'Не удалось покинуть проект', color: 'error' })
      return false
    }
  }

  const updateProjectDetails = async (projectId: string, data: UpdateProjectDetailsRequest): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}`, { method: 'PUT', body: data })
      toast.add({ title: 'Успех', description: 'Информация о проекте обновлена', color: 'success' })
      return true
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: error.data?.message || 'Не удалось обновить проект', color: 'error' })
      return false
    }
  }

  const changeMemberRole = async (projectId: string, userId: string, roleId: number): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}/members/${userId}/role`, { method: 'PUT', body: { newRole: roleId } })
      toast.add({ title: 'Успех', description: 'Роль участника изменена', color: 'success' })
      return true
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: error.data?.message || 'Не удалось изменить роль', color: 'error' })
      return false
    }
  }

  const removeMember = async (projectId: string, userId: string): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}/members/${userId}`, { method: 'DELETE' })
      toast.add({ title: 'Успех', description: 'Участник успешно удален из проекта', color: 'success' })
      return true
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: error.data?.message || 'Не удалось удалить участника', color: 'error' })
      return false
    }
  }

  const deleteProject = async (projectId: string): Promise<boolean> => {
    try {
      await api(`/api/projects/${projectId}`, { method: 'DELETE' })
      toast.add({ title: 'Успех', description: 'Проект полностью удален', color: 'success' })
      return true
    } catch (error: any) {
      toast.add({ title: 'Ошибка', description: error.data?.message || 'Не удалось удалить проект', color: 'error' })
      return false
    }
  }

  const setCurrentProject = async (projectId: string): Promise<void> => {
    if (projects.value.length === 0) {
      await fetchProjects()
    }
    const targetProject = projectItems.value.find(p => p.id === projectId)
    if (targetProject) {
      selectedProject.value = targetProject
    } else {
      console.warn(`Проект с ID ${projectId} не найден в списке доступных проектов пользователя.`)
    }
  }

  const clearStore = () => {
  projects.value = []
  selectedProject.value = undefined
}

  return {
    projects,
    projectItems,
    selectedProject,
    fetchProjects,
    createProject,
    getMembers,
    leaveProject,
    updateProjectDetails,
    changeMemberRole,
    removeMember,
    deleteProject,
    clearStore,
    setCurrentProject,
    canManageProject,
    canEditTasks,
    isObserver,
    inviteUser
  }
})