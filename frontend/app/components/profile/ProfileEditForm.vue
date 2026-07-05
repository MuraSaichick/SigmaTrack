<script setup lang="ts">
import { z } from 'zod'
import type { FormSubmitEvent } from '#ui/types'
import type { UserProfileResponse, UpdateProfileRequest } from '~/types/user'

const props = defineProps<{
    profile: UserProfileResponse
}>()
const emit = defineEmits(['save', 'cancel'])
const { t } = useI18n()

const schema = z.object({
    firstname: z.string().min(1, t('profile.validation.required')),
    lastname: z.string().min(1, t('profile.validation.required')),
    patronymic: z.string().nullable().optional(),
    phone: z.string().min(1, t('profile.validation.required')),
    statusMessage: z.string().nullable().optional(),
    bio: z.string().nullable().optional(),
    position: z.string().nullable().optional(),
    department: z.string().nullable().optional(),
    skills: z.array(z.string()),
    birthDate: z.string().nullable().optional(),
    telegram: z.string().nullable().optional(),
    github: z.string().nullable().optional()
})
const onSubmit = (event: FormSubmitEvent<any>) => {
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
        gitHub: event.data.github || null
    }
    emit('save', payload)
}
</script>