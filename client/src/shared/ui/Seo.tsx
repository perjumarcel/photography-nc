import { useEffect } from 'react';

interface SeoProps {
  title: string;
  description?: string;
  image?: string | null;
  canonicalPath?: string;
}

export function Seo({ title, description, image, canonicalPath }: SeoProps): null {
  useEffect(() => {
    document.title = title;
    upsertMeta('description', description);
    upsertProperty('og:title', title);
    upsertProperty('og:type', 'website');
    upsertProperty('og:description', description);
    upsertProperty('og:image', image ?? undefined);
    upsertProperty('twitter:card', image ? 'summary_large_image' : 'summary');
    upsertProperty('twitter:title', title);
    upsertProperty('twitter:description', description);
    upsertProperty('twitter:image', image ?? undefined);
    upsertCanonical(canonicalPath);
  }, [canonicalPath, description, image, title]);

  return null;
}

function upsertMeta(name: string, content: string | undefined): void {
  upsert(`meta[name="${name}"]`, () => {
    const el = document.createElement('meta');
    el.setAttribute('name', name);
    return el;
  }, content);
}

function upsertProperty(property: string, content: string | undefined): void {
  upsert(`meta[property="${property}"]`, () => {
    const el = document.createElement('meta');
    el.setAttribute('property', property);
    return el;
  }, content);
}

function upsertCanonical(path: string | undefined): void {
  const selector = 'link[rel="canonical"]';
  const existing = document.head.querySelector<HTMLLinkElement>(selector);
  if (!path) {
    existing?.remove();
    return;
  }
  const el = existing ?? document.createElement('link');
  el.setAttribute('rel', 'canonical');
  el.href = new URL(path, window.location.origin).toString();
  if (!existing) document.head.appendChild(el);
}

function upsert(selector: string, create: () => HTMLMetaElement, content: string | undefined): void {
  const existing = document.head.querySelector<HTMLMetaElement>(selector);
  if (!content) {
    existing?.remove();
    return;
  }
  const el = existing ?? create();
  el.content = content;
  if (!existing) document.head.appendChild(el);
}
