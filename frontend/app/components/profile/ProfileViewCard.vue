<script setup lang="ts">
import type { UserProfileResponse } from '~/types/user'
defineProps<{
  profile: UserProfileResponse
}>()
defineEmits(['edit'])
</script>
<template>
  <div class="space-y-6 bg-white dark:bg-gray-900 p-6 rounded-xl border border-gray-200 dark:border-gray-800 shadow-sm">
    <!-- Шапка профиля (Аватар + Имя + Статус) -->
    <div class="flex flex-col sm:flex-row items-center gap-4 border-b border-gray-100 dark:border-gray-800 pb-6">
      <UAvatar
        :alt="`${profile.firstname} ${profile.lastname}`"
        :chip-color="profile.statusMessage ? 'amber' : 'green'"
        chip-text=""
        size="xl"
        class="text-2xl font-bold"
      />
      <div class="text-center sm:text-left flex-1 space-y-1">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white">
          {{ profile.lastname }} {{ profile.firstname }} {{ profile.patronymic }}
        </h2>
        <p v-if="profile.position" class="text-sm text-gray-500 dark:text-gray-400 font-medium">
          {{ profile.position }} <span v-if="profile.department">· {{ profile.department }}</span>
        </p>
        <!-- Красивый блок статуса, о котором шла речь -->
        <div v-if="profile.statusMessage" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-amber-50 dark:bg-amber-950/40 text-amber-700 dark:text-amber-400 rounded-md text-xs font-medium">
          <UIcon name="i-heroicons-chat-bubble-bottom-center-text" class="w-3.5 h-3.5" />
          {{ profile.statusMessage }}
        </div>
      </div>
      <UButton icon="i-heroicons-pencil-square" color="neutral" variant="subtle" @click="$emit('edit')">
        {{ $t('profile.edit') }}
      </UButton>
    </div>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
      <div class="space-y-3">
        <div class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
          <UIcon name="i-heroicons-envelope" class="w-4 h-4 text-gray-400" />
          <span class="font-medium">Email:</span> {{ profile.email }}
        </div>
        <div class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
          <UIcon name="i-heroicons-phone" class="w-4 h-4 text-gray-400" />
          <span class="font-medium">{{ $t('profile.fields.phone') }}:</span> {{ profile.phone }}
        </div>
        <div v-if="profile.birthDate" class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
          <UIcon name="i-heroicons-calendar" class="w-4 h-4 text-gray-400" />
          <span class="font-medium">{{ $t('profile.fields.birthDate') }}:</span> 
          {{ new Date(profile.birthDate).toLocaleDateString() }}
        </div>
      </div>

      <div class="space-y-3">
        <div v-if="profile.telegram" class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
          <UIcon name="i-simple-icons-telegram" class="w-4 h-4 text-sky-500" />
          <span class="font-medium">Telegram:</span> 
          <a :href="`https://t.me{profile.telegram}`" target="_blank" class="text-primary hover:underline">@{{ profile.telegram }}</a>
        </div>
        <div v-if="profile.gitHub" class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
          <UIcon name="i-simple-icons-github" class="w-4 h-4 text-gray-900 dark:text-white" />
          <span class="font-medium">GitHub:</span> 
          <a :href="`https://github.com{profile.github}`" target="_blank" class="text-primary hover:underline">{{ profile.gitHub }}</a>
        </div>
      </div>
    </div>

    <div v-if="profile.bio" class="border-t border-gray-100 dark:border-gray-800 pt-4">
      <h3 class="text-sm font-semibold text-gray-900 dark:text-white mb-2">{{ $t('profile.fields.bio') }}</h3>
      <p class="text-sm text-gray-600 dark:text-gray-400 whitespace-pre-line">{{ profile.bio }}</p>
    </div>

    <div v-if="profile.skills?.length" class="border-t border-gray-100 dark:border-gray-800 pt-4">
      <h3 class="text-sm font-semibold text-gray-900 dark:text-white mb-2">{{ $t('profile.fields.skills') }}</h3>
      <div class="flex flex-wrap gap-1.5">
        <UBadge v-for="skill in profile.skills" :key="skill" color="primary" variant="subtle" size="sm">
          {{ skill }}
        </UBadge>
      </div>
    </div>
  </div>
</template>