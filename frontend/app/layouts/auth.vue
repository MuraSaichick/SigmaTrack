<script setup lang="ts">
const colorMode = useColorMode()
const isDark = computed({
  get() { return colorMode.value === 'dark' },
  set() { colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark' }
})

const { locale } = useI18n()
const toggleLanguage = () => {
  locale.value = locale.value === 'ru' ? 'en' : 'ru'
}
</script>

<template>
  <div
    class="min-h-screen flex flex-col items-center justify-center p-4 bg-radial from-neutral-100 to-neutral-200 dark:from-neutral-900 dark:to-neutral-950 transition-colors duration-300 relative selection:bg-primary/30">

    <div
      class="absolute top-6 right-6 flex items-center gap-2 bg-white/60 dark:bg-neutral-900/60 backdrop-blur-md border border-neutral-200/50 dark:border-neutral-800/50 p-1.5 rounded-full shadow-sm">
      <UButton color="neutral" variant="ghost" size="sm" class="rounded-full" icon="i-lucide-languages"
        @click="toggleLanguage">
        {{ locale.toUpperCase() }}
      </UButton>
      <UButton color="neutral" variant="ghost" size="sm" class="rounded-full"
        :icon="isDark ? 'i-lucide-moon' : 'i-lucide-sun'" @click="() => {isDark = !isDark}" />
    </div>

    <div class="w-full max-w-md space-y-6">
      <div class="text-center space-y-1.5">
        <h1 class="text-4xl font-black tracking-tight text-primary">
          SIGMA<span class="text-neutral-900 dark:text-white font-semibold">TRACK</span>
        </h1>
        <p class="text-neutral-500 dark:text-neutral-400 text-xs uppercase tracking-widest font-bold">
          {{ $t('auth.tagline') }}
        </p>
      </div>
      <div
        class="bg-white/90 dark:bg-neutral-900/90 backdrop-blur-lg border border-neutral-200/80 dark:border-neutral-800/80 rounded-3xl shadow-2xl p-8 transition-all duration-300">
        <slot />
      </div>
    </div>
  </div>
</template>