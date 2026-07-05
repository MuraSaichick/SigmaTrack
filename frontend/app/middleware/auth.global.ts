export default defineNuxtRouteMiddleware((to) => {
  const { isAuthenticated } = useAuth()

  const isAuthPage = to.path.startsWith('/auth')

  if (!isAuthenticated.value && !isAuthPage) {
    return navigateTo('/auth/login')
  }

  if (isAuthenticated.value && isAuthPage) {
    return navigateTo('/')
  }
})