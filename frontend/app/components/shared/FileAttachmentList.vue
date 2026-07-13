<template>
  <div class="space-y-2">
    <div
      v-for="item in files"
      :key="item.id"
      class="flex items-center justify-between p-3 bg-white border border-gray-200 rounded-xl shadow-sm text-sm"
    >
      <div class="flex items-center space-x-3 truncate max-w-[70%]">
        <div class="text-gray-400 shrink-0 text-lg">
          {{ getFileIcon(item.file.type || item.file.name) }}
        </div>

        <div class="flex flex-col truncate">
          <span class="font-medium text-gray-700 truncate">{{ item.file.name }}</span>
          <span class="text-xs text-gray-400">{{ formatBytes(item.file.size) }}</span>
        </div>
      </div>

      <div class="flex items-center space-x-3 shrink-0 ml-4">
        <div class="flex items-center">
          <span v-if="item.isUploading" class="text-blue-500 flex items-center space-x-1.5 text-xs font-medium animate-pulse">
            <svg class="animate-spin h-3.5 w-3.5 text-blue-500" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span>Загрузка...</span>
          </span>
          
          <span v-else-if="item.error" class="text-red-500 text-xs bg-red-50 px-2.5 py-1 rounded-md font-medium border border-red-100">
            {{ item.error }}
          </span>
          
          <span v-else class="text-green-600 text-xs bg-green-50 px-2.5 py-1 rounded-md font-medium border border-green-100 flex items-center space-x-1">
            <span>✓ Готово</span>
          </span>
        </div>

        <button
          type="button"
          @click="emit('remove', item.id)"
          class="text-gray-400 hover:text-red-500 hover:bg-gray-50 rounded-lg p-1.5 transition-colors"
          title="Удалить файл"
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-4 h-4">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { useFileUploader } from '~/composables/useFileUploader'

type UploadingFile = ReturnType<typeof useFileUploader>['filesList']['value'][number]

defineProps<{
  files: UploadingFile[]
}>()

const emit = defineEmits<{
  (e: 'remove', id: string): void
}>()

const formatBytes = (bytes: number, decimals = 2) => {
  if (bytes === 0) return '0 Байт'
  const k = 1024
  const dm = decimals < 0 ? 0 : decimals
  const sizes = ['Байт', 'КБ', 'МБ', 'ГБ']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i]
}

const getFileIcon = (mimeOrName: string): string => {
  const lower = mimeOrName.toLowerCase()
  if (lower.startsWith('image/')) return '🖼️'
  if (lower.startsWith('video/')) return '🎥'
  if (lower.includes('pdf')) return '📄'
  if (lower.includes('zip') || lower.includes('rar') || lower.includes('tar')) return '📦'
  if (lower.includes('doc') || lower.includes('docx') || lower.includes('txt')) return '📝'
  if (lower.includes('xls') || lower.includes('xlsx')) return '📊'
  return '📎'
}
</script>