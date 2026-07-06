<script setup lang="ts">
import type { RegisterRequest } from '~/types/auth'

definePageMeta({ layout: 'auth' })

const config = useRuntimeConfig()
const toast = useToast()
const { t } = useI18n()

const registerForm = ref<RegisterRequest>({
  login: '', email: '', phone: '', password: '', firstname: '', lastname: '', patronymic: ''
})
const confirmPassword = ref('')
const isLoading = ref(false)

const showPassword = ref(false)
const showConfirmPassword = ref(false)

const onRegisterSubmit = async () => {
  if (isLoading.value) return

  if (registerForm.value.password !== confirmPassword.value) {
    toast.add({ 
      title: t('auth.validation.error_title', 'Ошибка'), 
      description: t('auth.validation.password_mismatch'), 
      color: 'error' 
    })
    return
  }

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
    toast.add({ 
      title: 'Ошибка регистрации', 
      description: error.data?.message || 'Проверьте данные или попробуйте другой логин', 
      color: 'error' 
    })
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="space-y-4 max-w-xl mx-auto">
    <div class="text-center sm:text-left">
      <h2 class="text-2xl font-bold tracking-tight text-neutral-900 dark:text-white">
        {{ $t('auth.register_title') }}
      </h2>
      <p class="text-sm text-neutral-500 dark:text-neutral-400 mt-1">
      {{ $t('auth.register_subtitle') }}
      </p>
    </div>
    <form @submit.prevent="onRegisterSubmit" class="space-y-4">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UFormField :label="$t('fields.login')">
          <UInput v-model="registerForm.login" icon="i-heroicons-user-plus" class="w-full" :disabled="isLoading" required />
        </UFormField>
        
        <UFormField :label="$t('fields.email')">
          <UInput v-model="registerForm.email" type="email" icon="i-heroicons-envelope" class="w-full" :disabled="isLoading" required />
        </UFormField>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UFormField :label="$t('fields.password')">
          <UInput 
            v-model="registerForm.password" 
            :type="showPassword ? 'text' : 'password'" 
            icon="i-heroicons-key" 
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

        <UFormField :label="$t('fields.confirm_password', 'Подтвердите пароль')">
          <UInput 
            v-model="confirmPassword" 
            :type="showConfirmPassword ? 'text' : 'password'" 
            icon="i-heroicons-shield-check" 
            class="w-full" 
            :disabled="isLoading" 
            required
            :ui="{ trailing: 'pr-0' }"
          >
            <template #trailing>
              <UButton
                color="neutral"
                variant="ghost"
                :icon="showConfirmPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                class="hover:bg-transparent dark:hover:bg-transparent"
                @click="void (showConfirmPassword = !showConfirmPassword)"
              />
            </template>
          </UInput>
        </UFormField>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <UFormField :label="$t('fields.lastname')">
          <UInput v-model="registerForm.lastname" :disabled="isLoading" required />
        </UFormField>
        
        <UFormField :label="$t('fields.firstname')">
          <UInput v-model="registerForm.firstname" :disabled="isLoading" required />
        </UFormField>

        <UFormField :label="$t('fields.patronymic')">
          <UInput v-model="registerForm.patronymic" :disabled="isLoading" :placeholder="$t('fields.patronymic_placeholder')"/>
        </UFormField>
      </div>
      <UFormField :label="$t('fields.phone')">
        <UInput v-model="registerForm.phone" type="tel" icon="i-heroicons-phone" class="w-full" :disabled="isLoading" required />
      </UFormField>
      <UButton type="submit" color="primary" block size="lg" class="font-bold rounded-xl mt-2 shadow-sm transition-transform active:scale-[0.99]" :loading="isLoading">
        {{ $t('auth.register_btn') }}
      </UButton>
    </form>
    <div class="pt-3 border-t border-neutral-100 dark:border-neutral-800/60 text-center">
      <p class="text-neutral-500 dark:text-neutral-400 text-sm">
        {{ $t('auth.have_account') }}
        <NuxtLink to="/auth/login" class="text-primary font-bold hover:underline ml-1 transition-colors">
          {{ $t('auth.login_link') }}
        </NuxtLink>
      </p>
    </div>
  </div>
</template>