<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useSprintStore } from '~/stores/useSprintStore'
import type { CreateSprintDto } from '~/types/sprint'

const route = useRoute()
const router = useRouter()
const sprintStore = useSprintStore()

const projectId = computed(() => route.params.projectId as string)

const getTodayString = (offsetDays = 0) => {
  const date = new Date()
  date.setDate(date.getDate() + offsetDays)
  return date.toISOString().split('T')[0]
}

const formData = ref({
  name: '',
  goal: '',
  startDate: getTodayString(0),
  endDate: getTodayString(14),
  capacity: 80
})

const validateForm = (state: any) => {
  const errors: any[] = []
  if (!state.name) errors.push({ path: 'name', message: 'Название спринта обязательно.' })
  if (state.name && state.name.length > 150) errors.push({ path: 'name', message: 'Максимальная длина 150 символов.' })
  if (!state.startDate) errors.push({ path: 'startDate', message: 'Укажите дату начала.' })
  if (!state.endDate) errors.push({ path: 'endDate', message: 'Укажите дату окончания.' })
  if (state.startDate && state.endDate && new Date(state.startDate) >= new Date(state.endDate)) {
    errors.push({ path: 'endDate', message: 'Дата окончания должна быть позже даты начала.' })
  }
  if (!state.capacity || state.capacity <= 0) {
    errors.push({ path: 'capacity', message: 'Емкость должна быть больше 0.' })
  }
  return errors
}

const handleSubmit = async () => {
  const payload: CreateSprintDto = {
    name: formData.value.name,
    goal: formData.value.goal || null,
    startDate: new Date(formData.value.startDate || '').toISOString(),
    endDate: new Date(formData.value.endDate || '').toISOString(),
    capacity: formData.value.capacity
  }
  const success = await sprintStore.createSprint(projectId.value, payload)
  if (success) {
    router.push(`/projects/${projectId.value}/sprints`)
  }
}
</script>

<template>
  <div class="p-6 max-w-2xl mx-auto space-y-6">
    <div class="flex items-center gap-3 pb-4 border-b border-gray-100 dark:border-gray-800">
      <UButton 
        :to="`/projects/${projectId}/sprints`" 
        icon="i-heroicons-arrow-left" 
        color="neutral" 
        variant="ghost" 
        class="rounded-xl" 
      />
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Создание спринта</h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">Запланируйте новую рабочую итерацию проекта</p>
      </div>
    </div>

    <UAlert
      v-if="sprintStore.error"
      icon="i-heroicons-exclamation-triangle"
      color="error"
      variant="soft"
      title="Ошибка создания"
      :description="sprintStore.error"
      class="rounded-xl"
    />

    <UForm :state="formData" :validate="validateForm" @submit="handleSubmit" class="space-y-4">
      
      <UFormField label="Название спринта" name="name" required>
        <UInput 
          v-model="formData.name" 
          placeholder="Например, Спринт 1: Интеграция API" 
          maxlength="150"
          class="w-full"
        />
      </UFormField>

      <UFormField label="Цель спринта (необязательно)" name="goal">
        <UTextarea 
          v-model="formData.goal" 
          placeholder="Опишите основную бизнес-цель данной итерации..." 
          :rows="3"
          class="w-full"
        />
      </UFormField>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UFormField label="Дата начала" name="startDate" required>
          <UInput 
            v-model="formData.startDate" 
            type="date" 
            class="w-full"
          />
        </UFormField>

        <UFormField label="Дата окончания" name="endDate" required>
          <UInput 
            v-model="formData.endDate" 
            type="date" 
            class="w-full"
          />
        </UFormField>
      </div>

      <UFormField label="Емкость спринта (Capacity в Story Points)" name="capacity" required>
        <UInput 
          v-model.number="formData.capacity" 
          type="number" 
          min="1"
          placeholder="100"
          class="w-full"
        />
      </UFormField>

      <div class="flex items-center justify-end gap-3 pt-4 border-t border-gray-100 dark:border-gray-800">
        <UButton 
          :to="`/projects/${projectId}/sprints`" 
          color="neutral" 
          variant="outline"
          class="rounded-xl"
        >
          Отмена
        </UButton>
        <UButton 
          type="submit" 
          color="primary" 
          class="rounded-xl font-bold px-6"
          :loading="sprintStore.isLoading"
        >
          Создать спринт
        </UButton>
      </div>
    </UForm>
  </div>
</template>