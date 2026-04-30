import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { describe, expect, it } from 'vitest';

const sourceRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const importPattern = /(?:import|export)\s+(?:type\s+)?(?:[^'"]*?\s+from\s+)?['"]([^'"]+)['"]|import\(\s*['"]([^'"]+)['"]\s*\)/g;

describe('feature-sliced import boundaries', () => {
  it('keeps shared independent from features and pages', () => {
    const violations = collectSourceFiles(path.join(sourceRoot, 'shared'))
      .flatMap((filePath) => readImports(filePath)
        .filter((specifier) => specifier.startsWith('@/features/') || specifier.startsWith('@/pages/'))
        .map((specifier) => `${relative(filePath)} -> ${specifier}`));

    expect(violations).toEqual([]);
  });

  it('prevents imports between different features', () => {
    const featuresRoot = path.join(sourceRoot, 'features');
    const violations = collectSourceFiles(featuresRoot)
      .flatMap((filePath) => {
        const featureName = path.relative(featuresRoot, filePath).split(path.sep)[0];
        return readImports(filePath)
          .filter((specifier) => specifier.startsWith('@/features/'))
          .filter((specifier) => !specifier.startsWith(`@/features/${featureName}/`))
          .map((specifier) => `${relative(filePath)} -> ${specifier}`);
      });

    expect(violations).toEqual([]);
  });
});

function collectSourceFiles(directory: string): string[] {
  return fs.readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) return collectSourceFiles(fullPath);
    return /\.(ts|tsx)$/.test(entry.name) ? [fullPath] : [];
  });
}

function readImports(filePath: string): string[] {
  const text = fs.readFileSync(filePath, 'utf8');
  return Array.from(text.matchAll(importPattern), (match) => match[1] ?? match[2]).filter(Boolean);
}

function relative(filePath: string): string {
  return path.relative(sourceRoot, filePath).replaceAll(path.sep, '/');
}
