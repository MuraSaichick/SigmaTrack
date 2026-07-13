import type { AttachmentInput } from '~/types/attachment'

export type StorageFolder = 'Avatars' | 'Users' | 'Projects' | 'Issues' | 'Comments'

interface UploadingFile {
  id: string
  file: File
  progress: number
  isUploading: boolean
  error: string | null
  uploadedData: AttachmentInput | null
}

export const useFileUploader = (folder: StorageFolder) => {
  const apiFetch = useApi()
  
  const filesList = ref<UploadingFile[]>([])
  const isAnyUploading = computed(() => filesList.value.some(f => f.isUploading))

  const uploadFile = async (file: File) => {
    const id = Math.random().toString(36).substring(2, 9)
    const trackItem = reactive<UploadingFile>({
      id,
      file,
      progress: 0,
      isUploading: true,
      error: null,
      uploadedData: null
    })
    filesList.value.push(trackItem)

    const formData = new FormData()
    formData.append('file', file)

    try {
      const data = await apiFetch<AttachmentInput>('/api/files/upload', {
        method: 'POST',
        query: { folder }, 
        body: formData,
      })
      trackItem.uploadedData = data
      trackItem.progress = 100
    } catch (err: any) {
      trackItem.error = 'Ошибка при загрузке' + err.data?.message || 'Ошибка при загрузке' + err.statusMessage || 'Ошибка при загрузке'
      console.error(err)
    } finally {
      trackItem.isUploading = false
    }
  }

  const removeFile = (id: string) => {
    filesList.value = filesList.value.filter(f => f.id !== id)
  }

  const clearAll = () => {
    filesList.value = []
  }

  const resultAttachments = computed<AttachmentInput[]>(() => {
    return filesList.value
      .filter(f => f.uploadedData && !f.error)
      .map(f => f.uploadedData!)
  })

  return {
    filesList,
    isAnyUploading,
    resultAttachments,
    uploadFile,
    removeFile,
    clearAll
  }
}