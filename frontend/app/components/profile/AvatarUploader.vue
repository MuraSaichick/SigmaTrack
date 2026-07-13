<template>
    <div class="flex flex-col items-center gap-4">
        <div class="relative group">
            <UAvatar :src="fullAvatarUrl" alt="User Avatar" size="3xl"
                class="w-32 h-32 object-cover border-2 border-gray-200 dark:border-gray-800 shadow-sm" />

            <div v-if="isUploading"
                class="absolute inset-0 rounded-full bg-black/60 flex flex-col items-center justify-center text-white text-xs font-medium gap-1">
                <UIcon name="i-heroicons-arrow-path" class="animate-spin text-lg" />
                <span>Обновление...</span>
            </div>
        </div>

        <label>
            <UButton as="span" icon="i-heroicons-arrow-up-tray" color="primary" variant="solid" class="cursor-pointer"
                :loading="isUploading">
                Изменить фото
            </UButton>
            <input type="file" accept="image/jpeg,image/png,image/webp" class="hidden" :disabled="isUploading"
                @change="handleFileChange" />
        </label>

        <p v-if="errorMessage" class="text-red-500 dark:text-red-400 text-xs text-center max-w-[200px]">
            {{ errorMessage }}
        </p>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
interface UploadAvatarResponse {
    avatarUrl: string
}
interface ProblemDetails {
    detail?: string
    title?: string
    status?: number
}
const emit = defineEmits(['uploaded'])
const modelValue = defineModel<string | null | undefined>()

const isUploading = ref<boolean>(false)
const errorMessage = ref<string>('')

const apiFetch = useApi()
const config = useRuntimeConfig()

const fullAvatarUrl = computed<string>(() => {
    if (!modelValue.value) return '/images/default-avatar.png'
    const baseUrl = modelValue.value.startsWith('http')
        ? modelValue.value
        : `${config.public.apiBase}${modelValue.value}`
    return `${baseUrl}?t=${Date.now()}`
})

const handleFileChange = async (event: Event) => {
    const target = event.target as HTMLInputElement
    const file = target.files?.[0]
    if (!file) return

    errorMessage.value = ''
    isUploading.value = true

    const formData = new FormData()
    formData.append('file', file)

    try {
        const data = await apiFetch<UploadAvatarResponse>('/api/profile/avatar', {
            method: 'POST',
            body: formData
        })
        modelValue.value = data.avatarUrl
        emit('uploaded')
    } catch (error: any) {
        const errorData = error.data as ProblemDetails | undefined
        if (errorData?.detail) {
            errorMessage.value = errorData.detail
        } else {
            errorMessage.value = 'Не удалось загрузить изображение.'
        }
    } finally {
        isUploading.value = false
        target.value = ''
    }
}
</script>