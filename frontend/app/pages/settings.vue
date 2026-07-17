<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import { useSettingsStore } from '~/stores/useSettingsStore'
import type { PrivacySettings } from '~/types/settings'

const { t } = useI18n()
const settingsStore = useSettingsStore()
const toast = useToast()
const { user: authUser } = useAuth()

const activeTab = ref<'security' | 'privacy'>('security')

const contactVisibilityOptions = computed(() => [
  { label: t('settings.privacy.contactVisibility.Everyone'), value: 'Everyone' },
  { label: t('settings.privacy.contactVisibility.TeamOnly'), value: 'TeamOnly' },
  { label: t('settings.privacy.contactVisibility.Nobody'), value: 'Nobody' }
])

const birthDateVisibilityOptions = computed(() => [
  { label: t('settings.privacy.birthDateVisibility.FullDate'), value: 'FullDate' },
  { label: t('settings.privacy.birthDateVisibility.MonthAndDayOnly'), value: 'MonthAndDayOnly' },
  { label: t('settings.privacy.birthDateVisibility.Hidden'), value: 'Hidden' }
])

const invitationRestrictionOptions = computed(() => [
  { label: t('settings.privacy.invitationRestriction.Everyone'), value: 'Everyone' },
  { label: t('settings.privacy.invitationRestriction.TeamOnly'), value: 'TeamOnly' }
])

const securityForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
  email: authUser.value?.email || ''
})
const isSavingSecurity = ref(false)

async function handleUpdateSecurity() {
  if (securityForm.value.newPassword || securityForm.value.currentPassword) {
    if (securityForm.value.newPassword !== securityForm.value.confirmPassword) {
      toast.add({ title: 'Ошибка', description: 'Пароли не совпадают', color: 'error' })
      return
    }
  }
  isSavingSecurity.value = true
  try {
    const promises: Promise<any>[] = []
    if (securityForm.value.email && securityForm.value.email !== authUser.value?.email) {
      promises.push(settingsStore.updateEmail({ newEmail: securityForm.value.email }))
    }
    
    if (securityForm.value.newPassword) {
      promises.push(settingsStore.updatePassword({
        currentPassword: securityForm.value.currentPassword,
        newPassword: securityForm.value.newPassword,
        confirmPassword: securityForm.value.confirmPassword
      }))
    }
    await Promise.all(promises)
    if (securityForm.value.email && authUser.value) {
      authUser.value = {
        ...authUser.value,
        email: securityForm.value.email
      }
    }
    securityForm.value.currentPassword = ''
    securityForm.value.newPassword = ''
    securityForm.value.confirmPassword = ''
  } catch (error) {
  } finally {
    isSavingSecurity.value = false
  }
}

const privacyForm = ref<PrivacySettings>({
  showContacts: 'TeamOnly',
  showBirthDate: 'MonthAndDayOnly',
  showOnlineStatus: true,
  whoCanInviteMe: 'Everyone',
  searchable: true,
  showStatusMessage: true
})

onMounted(async () => {
  await settingsStore.fetchPrivacy()
  if (settingsStore.privacy) {
    privacyForm.value = { ...settingsStore.privacy }
  }
})

watch(() => settingsStore.privacy, (newVal) => {
  if (newVal) {
    privacyForm.value = { ...newVal }
  }
}, { deep: true })

async function handleUpdatePrivacy() {
  try {
    await settingsStore.updatePrivacy(privacyForm.value)
  } catch (error) {}
}
</script>
<template>
  <div class="max-w-6xl mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold tracking-tight text-neutral-900 dark:text-neutral-50">
        {{ $t('settings.title') }}
      </h1>
      <p class="mt-2 text-sm text-neutral-500 dark:text-neutral-400">
        {{ $t('settings.subtitle') }}
      </p>
    </div>

    <div class="flex flex-col md:flex-row gap-8">
      <aside class="w-full md:w-64 shrink-0">
        <nav class="flex md:flex-col gap-1 overflow-x-auto md:overflow-visible pb-2 md:pb-0">
          <UButton
            :variant="activeTab === 'security' ? 'solid' : 'ghost'"
            color="neutral"
            class="justify-start w-full text-left"
            icon="i-lucide-lock"
            @click="void (activeTab = 'security')"
          >
            {{ $t('settings.tabs.security') }}
          </UButton>
          
          <UButton
            :variant="activeTab === 'privacy' ? 'solid' : 'ghost'"
            color="neutral"
            class="justify-start w-full text-left"
            icon="i-lucide-eye-off"
            @click="void (activeTab = 'privacy')"
          >
            {{ $t('settings.tabs.privacy') }}
          </UButton>
        </nav>
      </aside>

      <main class="flex-1 bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-xl p-6 shadow-xs">
        
        <div v-if="activeTab === 'security'" class="space-y-6">
          <div>
            <h2 class="text-lg font-semibold text-neutral-900 dark:text-neutral-50">
              {{ $t('settings.security.title') }}
            </h2>
            <p class="text-xs text-neutral-500">
              {{ $t('settings.security.subtitle') }}
            </p>
          </div>
          
          <USeparator />

          <form @submit.prevent="handleUpdateSecurity" class="space-y-4">
            <UFormField 
              :label="$t('settings.security.emailLabel')" 
              :description="$t('settings.security.emailDesc')"
            >
              <UInput v-model="securityForm.email" type="email" icon="i-lucide-mail" class="w-full max-w-md" />
            </UFormField>

            <div class="pt-2">
              <h3 class="text-sm font-medium text-neutral-700 dark:text-neutral-300 mb-3">
                {{ $t('settings.security.passwordChangeTitle') }}
              </h3>
              <div class="space-y-4 max-w-md">
                <UFormField :label="$t('settings.security.currentPassword')">
                  <UInput v-model="securityForm.currentPassword" type="password" icon="i-lucide-key-round" />
                </UFormField>
                <UFormField :label="$t('settings.security.newPassword')">
                  <UInput v-model="securityForm.newPassword" type="password" icon="i-lucide-lock" />
                </UFormField>
                <UFormField :label="$t('settings.security.confirmPassword')">
                  <UInput v-model="securityForm.confirmPassword" type="password" icon="i-lucide-lock" />
                </UFormField>
              </div>
            </div>

            <div class="pt-4">
              <UButton type="submit" color="primary" :loading="isSavingSecurity">
                {{ $t('settings.security.saveButton') }}
              </UButton>
            </div>
          </form>
        </div>

        <div v-if="activeTab === 'privacy'" class="space-y-6">
          <div>
            <h2 class="text-lg font-semibold text-neutral-900 dark:text-neutral-50">
              {{ $t('settings.privacy.title') }}
            </h2>
            <p class="text-xs text-neutral-500">
              {{ $t('settings.privacy.subtitle') }}
            </p>
          </div>

          <USeparator />

          <form @submit.prevent="handleUpdatePrivacy" class="space-y-5">
            <UFormField 
              :label="$t('settings.privacy.contactsLabel')" 
              :description="$t('settings.privacy.contactsDesc')"
            >
              <USelect
                v-model="privacyForm.showContacts"
                :items="contactVisibilityOptions"
                class="w-full max-w-xs"
              />
            </UFormField>

            <UFormField 
              :label="$t('settings.privacy.birthDateLabel')" 
              :description="$t('settings.privacy.birthDateDesc')"
            >
              <USelect
                v-model="privacyForm.showBirthDate"
                :items="birthDateVisibilityOptions"
                class="w-full max-w-xs"
              />
            </UFormField>

            <UFormField 
              :label="$t('settings.privacy.invitationLabel')" 
              :description="$t('settings.privacy.invitationDesc')"
            >
              <USelect
                v-model="privacyForm.whoCanInviteMe"
                :items="invitationRestrictionOptions"
                class="w-full max-w-xs"
              />
            </UFormField>

            <div class="space-y-4 pt-2">
              <div class="flex items-center justify-between p-3 rounded-lg border border-neutral-100 dark:border-neutral-800">
                <div class="flex flex-col gap-0.5">
                  <span class="text-sm font-medium text-neutral-900 dark:text-neutral-50">
                    {{ $t('settings.privacy.onlineStatusTitle') }}
                  </span>
                  <span class="text-xs text-neutral-400">
                    {{ $t('settings.privacy.onlineStatusDesc') }}
                  </span>
                </div>
                <USwitch v-model="privacyForm.showOnlineStatus" color="primary" />
              </div>

              <div class="flex items-center justify-between p-3 rounded-lg border border-neutral-100 dark:border-neutral-800">
                <div class="flex flex-col gap-0.5">
                  <span class="text-sm font-medium text-neutral-900 dark:text-neutral-50">
                    {{ $t('settings.privacy.searchableTitle') }}
                  </span>
                  <span class="text-xs text-neutral-400">
                    {{ $t('settings.privacy.searchableDesc') }}
                  </span>
                </div>
                <USwitch v-model="privacyForm.searchable" color="primary" />
              </div>

              <div class="flex items-center justify-between p-3 rounded-lg border border-neutral-100 dark:border-neutral-800">
                <div class="flex flex-col gap-0.5">
                  <span class="text-sm font-medium text-neutral-900 dark:text-neutral-50">
                    {{ $t('settings.privacy.statusMsgTitle') }}
                  </span>
                  <span class="text-xs text-neutral-400">
                    {{ $t('settings.privacy.statusMsgDesc') }}
                  </span>
                </div>
                <USwitch v-model="privacyForm.showStatusMessage" color="primary" />
              </div>
            </div>

            <div class="pt-4">
              <UButton type="submit" color="primary" :loading="settingsStore.isLoading">
                {{ $t('settings.privacy.saveButton') }}
              </UButton>
            </div>
          </form>
        </div>

      </main>
    </div>
  </div>
</template>