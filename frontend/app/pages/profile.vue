<script setup lang="ts">
import { z } from 'zod'
import type { FormSubmitEvent } from '#ui/types'
import type { UpdateProfileRequest, UserProfileResponse } from '~/types/user'
import { useProfile } from '~/composables/useProfile'
import AvatarUploader from '~/components/profile/AvatarUploader.vue'

const { getProfile, updateProfile } = useProfile()
const auth = useAuth()
const toast = useToast()
const { t } = useI18n()

const isEditing = ref(false)
const newSkill = ref('')

const { data: profileData, error, refresh } = await useAsyncData<UserProfileResponse>('user-profile', () => getProfile())

    watch(() => profileData.value?.avatarUrl, (newUrl) => {
    if (newUrl && auth.user.value) {
        auth.user.value.avatarUrl = newUrl
    }
})

const schema = z.object({
    firstname: z.string().min(1, t('profile.validation.required')),
    lastname: z.string().min(1, t('profile.validation.required')),
    patronymic: z.string().optional(),
    phone: z.string().min(1, t('profile.validation.required')),
    statusMessage: z.string().optional(),
    bio: z.string().optional(),
    position: z.string().optional(),
    department: z.string().optional(),
    skills: z.array(z.string()),
    birthDate: z.string().optional(),
    telegram: z.string().optional(),
    gitHub: z.string().optional()
})

const state = reactive({
    firstname: '',
    lastname: '',
    patronymic: undefined as string | undefined,
    phone: '',
    statusMessage: undefined as string | undefined,
    bio: undefined as string | undefined,
    position: undefined as string | undefined,
    department: undefined as string | undefined,
    skills: [] as string[],
    birthDate: undefined as string | undefined,
    telegram: undefined as string | undefined,
    gitHub: undefined as string | undefined
})

const startEditing = () => {
    if (!profileData.value) return

    state.firstname = profileData.value.firstname
    state.lastname = profileData.value.lastname
    state.patronymic = profileData.value.patronymic ?? undefined
    state.phone = profileData.value.phone
    state.statusMessage = profileData.value.statusMessage ?? undefined
    state.bio = profileData.value.bio ?? undefined
    state.position = profileData.value.position ?? undefined
    state.department = profileData.value.department ?? undefined
    state.skills = [...profileData.value.skills]
    state.birthDate = profileData.value.birthDate ? profileData.value.birthDate.split('T')[0] : undefined
    state.telegram = profileData.value.telegram ?? undefined
    state.gitHub = profileData.value.gitHub ?? undefined

    isEditing.value = true
}

const addSkill = () => {
    const trimmed = newSkill.value.trim()
    if (trimmed && !state.skills.includes(trimmed)) {
        state.skills.push(trimmed)
        newSkill.value = ''
    }
}
const removeSkill = (index: number) => {
    state.skills.splice(index, 1)
}

const onSubmit = async (event: FormSubmitEvent<any>) => {
    try {
        const payload: UpdateProfileRequest = {
            firstname: event.data.firstname,
            lastname: event.data.lastname,
            patronymic: event.data.patronymic || null,
            phone: event.data.phone,
            statusMessage: event.data.statusMessage || null,
            bio: event.data.bio || null,
            position: event.data.position || null,
            department: event.data.department || null,
            skills: event.data.skills,
            birthDate: event.data.birthDate ? new Date(event.data.birthDate).toISOString() : null,
            telegram: event.data.telegram || null,
            gitHub: event.data.gitHub || null
        }

        await updateProfile(payload)
        await refresh()

        if (profileData.value && auth.user.value) {
            auth.user.value.firstname = profileData.value.firstname
        }

        isEditing.value = false
        toast.add({ title: t('profile.successUpdate'), color: 'success' })
    } catch (err: any) {
        toast.add({
            title: 'Ошибка',
            description: err.data?.message || 'Не удалось обновить профиль',
            color: 'error'
        })
    }
}
</script>
<template>
    <div class="max-w-4xl mx-auto py-8 px-4">
        <div v-if="error" class="text-center py-12">
            <UAlert title="Ошибка загрузки данных" color="error" variant="subtle"
                icon="i-heroicons-exclamation-triangle" />
        </div>
        <div v-else-if="profileData">
            <UCard v-if="!isEditing" class="shadow-sm">
                <template #header>
                    <div class="flex flex-col sm:flex-row items-center gap-4">
                        <AvatarUploader v-model="profileData.avatarUrl" @uploaded="refresh"/>
                        <div class="text-center sm:text-left flex-1 space-y-1">
                            <h2 class="text-xl font-bold text-gray-900 dark:text-white">
                                {{ profileData.lastname }} {{ profileData.firstname }} {{ profileData.patronymic }}
                            </h2>
                            <p v-if="profileData.position" class="text-sm text-gray-500 dark:text-gray-400 font-medium">
                                {{ profileData.position }} <span v-if="profileData.department">· {{
                                    profileData.department }}</span>
                            </p>
                            <div v-if="profileData.statusMessage"
                                class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-amber-50 dark:bg-amber-950/40 text-amber-700 dark:text-amber-400 rounded-md text-xs font-medium">
                                <UIcon name="i-heroicons-chat-bubble-bottom-center-text" class="w-3.5 h-3.5" />
                                {{ profileData.statusMessage }}
                            </div>
                        </div>
                        <UButton icon="i-heroicons-pencil-square" color="neutral" variant="subtle"
                            @click="startEditing">
                            {{ $t('profile.edit') }}
                        </UButton>
                    </div>
                </template>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6 text-sm mb-6">
                    <div class="space-y-3">
                        <div class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-heroicons-user" class="w-4 h-4 text-gray-400" />
                            <span class="font-medium">Login:</span> {{ profileData.login }}
                        </div>
                        <div class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-heroicons-envelope" class="w-4 h-4 text-gray-400" />
                            <span class="font-medium">Email:</span> {{ profileData.email }}
                        </div>
                        <div class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-heroicons-phone" class="w-4 h-4 text-gray-400" />
                            <span class="font-medium">{{ $t('profile.fields.phone') }}:</span> {{ profileData.phone }}
                        </div>
                    </div>

                    <div class="space-y-3">
                        <div v-if="profileData.birthDate"
                            class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-heroicons-calendar" class="w-4 h-4 text-gray-400" />
                            <span class="font-medium">{{ $t('profile.fields.birthDate') }}:</span>
                            {{ new Date(profileData.birthDate).toLocaleDateString() }}
                        </div>
                        <div v-if="profileData.telegram"
                            class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-simple-icons-telegram" class="w-4 h-4 text-sky-500" />
                            <span class="font-medium">{{ $t('profile.fields.telegram') }}:</span>
                            <a :href="`https://t.me/${profileData.telegram}`" target="_blank"
                                class="text-primary hover:underline">@{{
                                    profileData.telegram }}</a>
                        </div>
                        <div v-if="profileData.gitHub" class="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <UIcon name="i-simple-icons-github" class="w-4 h-4 text-gray-900 dark:text-white" />
                            <span class="font-medium">{{ $t('profile.fields.github') }}:</span>
                            <a :href="`https://github.com/${profileData.gitHub}`" target="_blank"
                                class="text-primary hover:underline">{{
                                    profileData.gitHub }}</a>
                        </div>
                    </div>
                </div>
                <div v-if="profileData.bio" class="border-t border-gray-100 dark:border-gray-800 pt-4 mb-6">
                    <h3 class="text-sm font-semibold text-gray-900 dark:text-white mb-2">{{ $t('profile.fields.bio') }}
                    </h3>
                    <p class="text-sm text-gray-600 dark:text-gray-400 whitespace-pre-line">{{ profileData.bio }}</p>
                </div>
                <div v-if="profileData.skills.length" class="border-t border-gray-100 dark:border-gray-800 pt-4">
                    <h3 class="text-sm font-semibold text-gray-900 dark:text-white mb-2">{{ $t('profile.fields.skills')
                    }}</h3>
                    <div class="flex flex-wrap gap-1.5">
                        <UBadge v-for="skill in profileData.skills" :key="skill" color="primary" variant="subtle"
                            size="sm">
                            {{ skill }}
                        </UBadge>
                    </div>
                </div>
            </UCard>
            <UForm v-else :schema="schema" :state="state"
                class="space-y-6 bg-white dark:bg-gray-900 p-6 rounded-xl border border-gray-200 dark:border-gray-800 shadow-sm"
                @submit="onSubmit">
                <h2
                    class="text-xl font-bold text-gray-900 dark:text-white border-b border-gray-100 dark:border-gray-800 pb-3">
                    {{ $t('profile.title') }}
                </h2>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <UFormField :label="$t('profile.fields.lastname')" name="lastname" required>
                        <UInput v-model="state.lastname" class="w-full" />
                    </UFormField>

                    <UFormField :label="$t('profile.fields.firstname')" name="firstname" required>
                        <UInput v-model="state.firstname" class="w-full" />
                    </UFormField>

                    <UFormField :label="$t('profile.fields.patronymic')" name="patronymic">
                        <UInput v-model="state.patronymic" class="w-full" />
                    </UFormField>
                </div>

                <!-- Телефон + Дата Рождения -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <UFormField :label="$t('profile.fields.phone')" name="phone" required>
                        <UInput v-model="state.phone" icon="i-heroicons-phone" class="w-full" />
                    </UFormField>

                    <UFormField :label="$t('profile.fields.birthDate')" name="birthDate">
                        <UInput v-model="state.birthDate" type="date" icon="i-heroicons-calendar" class="w-full" />
                    </UFormField>
                </div>

                <!-- Текстовый статус сотрудника -->
                <UFormField :label="$t('profile.fields.statusMessage')" name="statusMessage">
                    <UInput v-model="state.statusMessage" :placeholder="$t('profile.fields.statusPlaceholder')"
                        icon="i-heroicons-face-smile" class="w-full" />
                </UFormField>

                <!-- Должность + Отдел -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <UFormField :label="$t('profile.fields.position')" name="position">
                        <UInput v-model="state.position" icon="i-heroicons-briefcase" class="w-full" />
                    </UFormField>

                    <UFormField :label="$t('profile.fields.department')" name="department">
                        <UInput v-model="state.department" icon="i-heroicons-academic-cap" class="w-full" />
                    </UFormField>
                </div>

                <!-- Соцсети (Telegram, GitHub) -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <UFormField :label="$t('profile.fields.telegram')" name="telegram">
                        <UInput v-model="state.telegram" icon="i-simple-icons-telegram" placeholder="username"
                            class="w-full" />
                    </UFormField>

                    <UFormField :label="$t('profile.fields.github')" name="gitHub">
                        <UInput v-model="state.gitHub" icon="i-simple-icons-github" placeholder="username"
                            class="w-full" />
                    </UFormField>
                </div>

                <!-- Биография -->
                <UFormField :label="$t('profile.fields.bio')" name="bio">
                    <UTextarea v-model="state.bio" :rows="3" class="w-full" />
                </UFormField>

                <!-- Динамическое добавление навыков -->
                <UFormField :label="$t('profile.fields.skills')" name="skills">
                    <div class="space-y-2 w-full">
                        <UInput v-model="newSkill" icon="i-heroicons-plus"
                            placeholder="Добавить навык и нажать Enter..." @keydown.enter.prevent="addSkill" />
                        <div class="flex flex-wrap gap-1.5 pt-1">
                            <UBadge v-for="(skill, index) in state.skills" :key="skill" color="primary"
                                variant="subtle">
                                {{ skill }}
                                <button type="button"
                                    class="ml-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 focus:outline-none"
                                    @click="removeSkill(index)">
                                    ×
                                </button>
                            </UBadge>
                        </div>
                    </div>
                </UFormField>
                <div class="flex justify-end gap-3 border-t border-gray-100 dark:border-gray-800 pt-4">
                    <UButton color="neutral" variant="ghost" @click="void (isEditing = false)">
                        {{ $t('profile.cancel') }}
                    </UButton>
                    <UButton type="submit" color="primary">
                        {{ $t('profile.save') }}
                    </UButton>
                </div>
            </UForm>
        </div>
    </div>
</template>