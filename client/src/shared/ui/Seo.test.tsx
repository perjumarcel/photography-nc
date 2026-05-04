import { render } from '@testing-library/react';
import { afterEach, describe, expect, it } from 'vitest';
import { Seo } from './Seo';

describe('Seo', () => {
  afterEach(() => {
    document.head.innerHTML = '';
  });

  it('uses name attributes for Twitter cards and property attributes for Open Graph', () => {
    render(<Seo title="Portfolio" description="Recent albums" image="/cover.jpg" canonicalPath="/portfolio" />);

    expect(document.head.querySelector('meta[property="og:title"]')?.getAttribute('content')).toBe('Portfolio');
    expect(document.head.querySelector('meta[name="twitter:title"]')?.getAttribute('content')).toBe('Portfolio');
    expect(document.head.querySelector('meta[property="twitter:title"]')).toBeNull();
    expect(document.head.querySelector('link[rel="canonical"]')?.getAttribute('href')).toBe('http://localhost:3000/portfolio');
  });
});
