export const useApi = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase
  const { token, logout } = useAuth() 

  const apiFetch = $fetch.create({
    baseURL: apiBase,
    onRequest({ options }) {
      if (token.value) {
        options.headers = options.headers || {}
        const headers = new Headers(options.headers)
        headers.set('Authorization', `Bearer ${token.value}`)
        options.headers = headers
      }
    },
    onResponseError({ response }) {
      if (response.status === 401) {
        logout()
      }
    }
  })
  return apiFetch
}
