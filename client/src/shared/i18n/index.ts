import i18n from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';

const resources = {
  ro: {
    translation: {
      'app.title': 'Fotografie',
      'nav.portfolio': 'Portofoliu',
      'nav.about': 'Despre',
      'nav.contact': 'Contact',
      'album.empty': 'Nu există albume.',
      'album.loading': 'Se încarcă...',
      'album.error': 'A apărut o eroare la încărcarea albumelor.',
      'admin.upload': 'Încarcă imagine',
      'admin.cover': 'Setează ca copertă',
    },
  },
  en: {
    translation: {
      'app.title': 'Photography',
      'nav.portfolio': 'Portfolio',
      'nav.about': 'About',
      'nav.contact': 'Contact',
      'album.empty': 'No albums yet.',
      'album.loading': 'Loading...',
      'album.error': 'Failed to load albums.',
      'admin.upload': 'Upload image',
      'admin.cover': 'Set as cover',
    },
  },
};

void i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: 'ro',
    supportedLngs: ['ro', 'en'],
    interpolation: { escapeValue: false },
    detection: { order: ['localStorage', 'navigator'] },
  });

export default i18n;
