import type { LoginResponse } from '~/types/auth'
import { useProjectsStore } from '~/stores/projects'

export const useAuth = () => {
    const token = useCookie<string | null>('auth_token', { maxAge: 60 * 60 * 24 * 7 })
    const user = useCookie<Omit<LoginResponse, 'token'> | null>('auth_user', { maxAge: 60 * 60 * 24 * 7 })

    const isAuthenticated = computed(() => !!token.value)

    const setSession = (authData: LoginResponse) => {
        token.value = authData.token
        user.value = {
            id: authData.id,
            login: authData.login,
            email: authData.email,
            firstname: authData.firstname
        }
    }
    const logout = () => {
        const projectsStore = useProjectsStore()
        projectsStore.clearStore()
        token.value = null
        user.value = null
        navigateTo('/auth/login')
    }

    return {
        token,
        user,
        isAuthenticated,
        setSession,
        logout
    }
}