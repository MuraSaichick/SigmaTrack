<script setup lang="ts">
import type { RegisterRequest } from '~/types/auth'

definePageMeta({ layout: 'auth' })

const config = useRuntimeConfig()
const toast = useToast()

const registerForm = ref<RegisterRequest>({
  login: '', email: '', phone: '', password: '', firstname: '', lastname: '', patronymic: ''
})
const isLoading = ref(false)

const onRegisterSubmit = async () => {
  if (isLoading.value) return
  isLoading.value = true
  try {
    await $fetch('/api/auth/register', {
      baseURL: config.public.apiBase as string,
      method: 'POST',
      body: registerForm.value
    })

    toast.add({ title: 'SigmaTrack', description: 'Регистрация успешна! Войдите в аккаунт.', color: 'primary' })
    await navigateTo('/auth/login')
  } catch (error: any) {
    toast.add({ title: 'Ошибка регистрации', description: 'Проверьте данные или попробуйте другой логин', color: 'error' })
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-2xl font-bold text-neutral-900 dark:text-white">
        {{ $t('auth.register_title') }}
      </h2>
    </div>

    <form @submit.prevent="onRegisterSubmit" class="space-y-4 max-h-[55vh] overflow-y-auto pr-1 scrollbar-thin">
      <UFormField :label="$t('fields.login')">
        <UInput v-model="registerForm.login" icon="i-lucide-user-plus" class="w-full" :disabled="isLoading" required />
      </UFormField>
      
      <UFormField :label="$t('fields.password')">
        <UInput v-model="registerForm.password" type="password" icon="i-lucide-key-round" class="w-full" :disabled="isLoading" required />
      </UFormField>

      <div class="grid grid-cols-2 gap-4">
        <UFormField :label="$t('fields.firstname')">
          <UInput v-model="registerForm.firstname" :disabled="isLoading" required />
        </UFormField>
        <UFormField :label="$t('fields.lastname')">
          <UInput v-model="registerForm.lastname" :disabled="isLoading" required />
        </UFormField>
      </div>

      <UFormField :label="$t('fields.patronymic')">
        <UInput v-model="registerForm.patronymic" :disabled="isLoading" />
      </UFormField>

      <UFormField :label="$t('fields.email')">
        <UInput v-model="registerForm.email" type="email" icon="i-lucide-mail" class="w-full" :disabled="isLoading" required />
      </UFormField>

      <UFormField :label="$t('fields.phone')">
        <UInput v-model="registerForm.phone" type="tel" icon="i-lucide-phone" class="w-full" :disabled="isLoading" required />
      </UFormField>

      <UButton type="submit" color="primary" block size="lg" class="font-bold rounded-xl mt-2" :loading="isLoading">
        {{ $t('auth.register_btn') }}
      </UButton>
    </form>

    <div class="pt-4 border-t border-neutral-100 dark:border-neutral-800/60 text-center">
      <p class="text-neutral-500 dark:text-neutral-400 text-sm">
        {{ $t('auth.have_account') }}
        <NuxtLink to="/auth/login" class="text-primary font-bold hover:underline ml-1 transition-colors">
          {{ $t('auth.login_link') }}
        </NuxtLink>
      </p>
    </div>
  </div>
</template>