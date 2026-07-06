import ru from './locales/ru'
import en from './locales/en'


export default defineI18nConfig(() => ({
  legacy: false,
  defaultLocale: 'ru',
  detectBrowserLanguage: {
    useCookie: true,
    cookieKey: 'i18n_redirected',
    redirectOn: 'root'
  },
  messages: {
    ru,
    en
  }
}))