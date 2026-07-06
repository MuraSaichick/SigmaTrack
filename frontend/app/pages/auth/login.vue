<script setup lang="ts">
import type { LoginRequest, LoginResponse } from '~/types/auth'

definePageMeta({ layout: 'auth' })

const { t } = useI18n()
const config = useRuntimeConfig()
const toast = useToast()
const { setSession } = useAuth()

const loginForm = ref<LoginRequest>({ login: '', password: '' })
const isLoading = ref(false) 
const showPassword = ref(false)

const onLoginSubmit = async () => {
  if (isLoading.value) return
  isLoading.value = true
  
  try {
    const response = await $fetch<LoginResponse>('/api/auth/login', {
      baseURL: config.public.apiBase as string,
      method: 'POST',
      body: loginForm.value
    })
    
    setSession(response)
    toast.add({ title: 'SigmaTrack', description: `${response.firstname}, добро пожаловать!`, color: 'primary' })
    await navigateTo('/')
  } catch (error: any) {
    toast.add({ title: 'Ошибка', description: 'Неверный логин или пароль', color: 'error' })
  } finally {
    isLoading.value = false 
  }
}
</script>

<template>
  <div class="space-y-6 max-w-md mx-auto">
    <div>
      <h2 class="text-2xl font-bold tracking-tight text-neutral-900 dark:text-white">
        {{ $t('auth.login_title') }}
      </h2>
    </div>
    
    <form @submit.prevent="onLoginSubmit" class="space-y-4">
      <UFormField :label="$t('fields.login')">
        <UInput v-model="loginForm.login" icon="i-heroicons-user" class="w-full" :disabled="isLoading" required />
      </UFormField>

      <UFormField :label="$t('fields.password')">
        <UInput 
          v-model="loginForm.password" 
          :type="showPassword ? 'text' : 'password'" 
          icon="i-heroicons-lock-closed" 
          class="w-full" 
          :disabled="isLoading" 
          required
          :ui="{ trailing: 'pr-0' }"
        >
          <template #trailing>
            <UButton
              color="neutral"
              variant="ghost"
              :icon="showPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
              class="hover:bg-transparent dark:hover:bg-transparent"
              @click="void (showPassword = !showPassword)"
            />
          </template>
        </UInput>
      </UFormField>

      <UButton type="submit" color="primary" block size="lg" class="font-bold rounded-xl shadow-sm transition-transform active:scale-[0.99]" :loading="isLoading">
        {{ $t('auth.login_btn') }}
      </UButton>
    </form>

    <div class="pt-4 border-t border-neutral-100 dark:border-neutral-800/60 text-center">
      <p class="text-neutral-500 dark:text-neutral-400 text-sm">
        {{ $t('auth.no_account') }}
        <NuxtLink to="/auth/register" class="text-primary font-bold hover:underline ml-1 transition-colors">
          {{ $t('auth.register_link') }}
        </NuxtLink>
      </p>
    </div>
  </div>
</template>