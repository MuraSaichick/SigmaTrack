<script setup lang="ts">
import { ref, watch } from 'vue'
import { useAuth } from '~/composables/useAuth'
import type { UserSearchDto, SearchUsersResponse } from '~/types/user'

const { isAuthenticated, user: currentUser } = useAuth()
const api = useApi()
const config = useRuntimeConfig()

const isOpen = ref(false)
const query = ref('')
const isLoading = ref(false)
const results = ref<UserSearchDto[]>([])

let debounceTimeout: number | undefined

const vFocus = {
  mounted: (el: HTMLElement) => {
    setTimeout(() => el.focus(), 50)
  }
}

watch(query, (newQuery) => {
  if (debounceTimeout) {
    clearTimeout(debounceTimeout)
  }

  const trimmed = newQuery.trim()
  if (trimmed.length < 2) {
    results.value = []
    return
  }

  isLoading.value = true
  debounceTimeout = window.setTimeout(async () => {
    try {
      const response = await api<SearchUsersResponse>('/api/users/search', {
        query: {
          query: trimmed,
          page: 1,
          pageSize: 10
        }
      })
      results.value = response.users.filter(u => u.id !== currentUser.value?.id)
    } catch (error) {
      results.value = []
    } finally {
      isLoading.value = false
    }
  }, 300)
})

defineShortcuts({
  meta_k: {
    usingInput: true,
    handler: () => {
      if (isAuthenticated.value) {
        isOpen.value = !isOpen.value
      }
    }
  }
})

function handleSelectUser(userId: string) {
  isOpen.value = false
  query.value = ''
  results.value = []
  navigateTo(`/users/${userId}`)
}

function getAvatarUrl(avatarUrl?: string | null) {
  if (!avatarUrl) return undefined
  if (avatarUrl.startsWith('http')) return avatarUrl
  return `${config.public.apiBase}${avatarUrl}`
}
</script>

<template>
  <div v-if="isAuthenticated" class="w-full">
    <UModal v-model:open="isOpen">
      <template #default>
        <UButton
          color="neutral"
          variant="subtle"
          icon="i-lucide-search"
          class="w-full justify-between text-neutral-400 dark:text-neutral-500"
        >
          <span class="text-xs sm:text-sm">Поиск коллег...</span>
          <UKbd size="md">⌘K</UKbd>
        </UButton>
      </template>

      <template #content>
        <UCard 
          :ui="{ body: 'p-0' }" 
          class="w-full max-w-md bg-white dark:bg-neutral-950 rounded-2xl border border-neutral-200 dark:border-neutral-800"
        >
          <div class="p-4 border-b border-neutral-100 dark:border-neutral-800 flex items-center gap-3">
            <UIcon name="i-lucide-search" class="w-5 h-5 text-neutral-400 shrink-0" />
            <input 
              v-model="query" 
              type="text" 
              placeholder="Введите логин, имя или фамилию..."
              class="w-full bg-transparent border-0 focus:ring-0 outline-hidden text-sm text-neutral-900 dark:text-neutral-50 placeholder-neutral-400 dark:placeholder-neutral-500"
              v-focus 
            />
          </div>

          <div class="max-h-96 overflow-y-auto p-2 space-y-1">
            <div v-if="isLoading" class="p-6 text-center text-sm text-neutral-400">
              <UIcon name="i-lucide-loader-2" class="animate-spin w-6 h-6 mx-auto mb-2 text-primary" />
              <span>Ищем на платформе...</span>
            </div>

            <template v-else-if="results.length > 0">
              <button 
                v-for="user in results" 
                :key="user.id" 
                @click="handleSelectUser(user.id)"
                class="w-full flex items-center gap-3 p-2 rounded-lg hover:bg-neutral-50 dark:hover:bg-neutral-800 text-left transition-colors"
              >
                <UAvatar 
                  :src="getAvatarUrl(user.avatarUrl)" 
                  :alt="`${user.firstname} ${user.lastname}`" 
                  size="sm" 
                />
                <div class="flex-1 min-w-0">
                  <div class="text-sm font-semibold text-neutral-900 dark:text-neutral-50 truncate">
                    {{ user.firstname }} {{ user.lastname }}
                  </div>
                  <div class="text-xs text-neutral-400 truncate">@{{ user.login }}</div>
                </div>
              </button>
            </template>

            <div v-else-if="query.trim().length >= 2" class="p-6 text-center text-sm text-neutral-400">
              <UIcon name="i-lucide-user-x" class="w-8 h-8 mx-auto mb-2 text-neutral-300 dark:text-neutral-700" />
              <p class="font-medium">Пользователи не найдены</p>
              <p class="text-xs text-neutral-400 mt-1">Попробуйте ввести другие ключевые слова</p>
            </div>

            <div v-else class="p-6 text-center text-xs text-neutral-400">
              <UIcon name="i-lucide-keyboard" class="w-8 h-8 mx-auto mb-2 text-neutral-300 dark:text-neutral-700" />
              <span>Введите хотя бы 2 символа для начала поиска</span>
            </div>
          </div>
        </UCard>
      </template>
    </UModal>
  </div>
</template>