// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  future: {
    compatibilityVersion: 4,
  },
  css: ['~/assets/css/main.css'],
  modules: [
    '@nuxt/ui',
    '@nuxtjs/i18n',
    '@pinia/nuxt'
  ],
  runtimeConfig: {
    public: {
      apiBase: ''
    }
  },
  i18n: {
    strategy: 'no_prefix',
    defaultLocale: 'ru',
    locales: [
      { code: 'ru', name: 'Русский' },
      { code: 'en', name: 'English' }
    ],
    vueI18n: 'D:/Projects/SigmaTrack/Project/frontend/i18n/i18n.config.ts'
  },
  devtools: { enabled: true }
})