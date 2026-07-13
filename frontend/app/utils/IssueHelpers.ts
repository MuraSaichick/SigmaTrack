export type NuxtUiColor = "primary" | "secondary" | "success" | "info" | "warning" | "error" | "neutral"

interface StatusConfig {
    label: string
    color: NuxtUiColor
    icon: string
}

export const getStatusConfig = (status: number): StatusConfig => {
    const map: Record<number, StatusConfig> = {
        0: { label: 'Бэклог', color: 'neutral', icon: 'i-heroicons-archive-box' },
        1: { label: 'К выполнению', color: 'info', icon: 'i-heroicons-clipboard' },
        2: { label: 'В работе', color: 'primary', icon: 'i-heroicons-arrow-path' },
        3: { label: 'Ревью', color: 'warning', icon: 'i-heroicons-eye' },
        4: { label: 'Тестирование', color: 'warning', icon: 'i-heroicons-beaker' },
        5: { label: 'Решена', color: 'success', icon: 'i-heroicons-check-circle' },
        6: { label: 'Закрыта', color: 'neutral', icon: 'i-heroicons-lock-closed' },
        7: { label: 'Переоткрыта', color: 'error', icon: 'i-heroicons-arrow-uturn-left' },
        8: { label: 'Отклонена', color: 'error', icon: 'i-heroicons-x-circle' },
        9: { label: 'Заморожена', color: 'secondary', icon: 'i-heroicons-pause-circle' }
    }

    return map[status] || { label: 'Неизвестно', color: 'neutral', icon: 'i-heroicons-question-mark-circle' }
}

interface PriorityConfig {
    label: string
    color: NuxtUiColor
    icon: string
}

export const getPriorityConfig = (priority: number): PriorityConfig => {
    const map: Record<number, PriorityConfig> = {
        0: { label: 'Критический', color: 'error', icon: 'i-heroicons-chevron-double-up' },
        1: { label: 'Высокий', color: 'warning', icon: 'i-heroicons-chevron-up' },
        2: { label: 'Средний', color: 'primary', icon: 'i-heroicons-bars-2' }, 
        3: { label: 'Низкий', color: 'info', icon: 'i-heroicons-chevron-down' },
        4: { label: 'Trivial', color: 'neutral', icon: 'i-heroicons-chevron-double-down' }
    }
    return map[priority] || { label: 'Обычный', color: 'neutral', icon: 'i-heroicons-minus' }
}

interface TypeConfig {
    label: string
    icon: string
    color: NuxtUiColor
}

export const getTypeConfig = (type: number): TypeConfig => {
    const map: Record<number, TypeConfig> = {
        0: { label: 'Баг', icon: 'i-heroicons-bug-ant', color: 'error' },
        1: { label: 'Фича', icon: 'i-heroicons-sparkles', color: 'primary' },
        2: { label: 'Улучшение', icon: 'i-heroicons-trending-up', color: 'secondary' },
        3: { label: 'Задача', icon: 'i-heroicons-check-badge', color: 'info' }
    }
    return map[type] || { label: 'Задача', icon: 'i-heroicons-document', color: 'neutral' }
}
