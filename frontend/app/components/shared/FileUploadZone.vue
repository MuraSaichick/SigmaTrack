<template>
  <div
    class="relative border-2 border-dashed rounded-xl p-6 text-center transition-all duration-200 cursor-pointer"
    :class="[
      isDragging 
        ? 'border-blue-500 bg-blue-50/50' 
        : 'border-gray-300 hover:border-gray-400 bg-gray-50/30 hover:bg-gray-50/80'
    ]"
    @dragover.prevent="onDragOver"
    @dragleave.prevent="onDragLeave"
    @drop.prevent="onDrop"
    @click="triggerFileInput"
  >
    <input
      ref="fileInputRef"
      type="file"
      multiple
      class="hidden"
      @change="onFileSelect"
    />

    <div class="flex flex-col items-center justify-center space-y-2 pointer-events-none">
      <div 
        class="p-2.5 rounded-full text-gray-500 transition-colors"
        :class="isDragging ? 'bg-blue-100 text-blue-600' : 'bg-gray-100'"
      >
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 16.5V9.75m0 0l3 3m-3-3l-3 3M6.75 19.5a4.5 4.5 0 01-1.41-8.775 5.25 5.25 0 0110.233-2.33 3 3 0 013.758 3.848A3.752 3.752 0 0118 19.5H6.75z" />
        </svg>
      </div>

      <div class="text-sm text-gray-600">
        <span class="font-semibold text-blue-600 hover:text-blue-700">Выберите файл</span> 
        или перетащите его из проводника
      </div>
      <p class="text-xs text-gray-400">Документы, изображения, архивы до 10 МБ</p>
    </div>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits<{
  (e: 'files-selected', files: File[]): void
}>()

const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)

const triggerFileInput = () => {
  fileInputRef.value?.click()
}

const onFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files?.length) {
    emit('files-selected', Array.from(target.files))
    target.value = ''
  }
}

const onDragOver = () => {
  isDragging.value = true
}

const onDragLeave = () => {
  isDragging.value = false
}

const onDrop = (event: DragEvent) => {
  isDragging.value = false
  if (event.dataTransfer?.files?.length) {
    emit('files-selected', Array.from(event.dataTransfer.files))
  }
}
</script>