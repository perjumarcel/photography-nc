import { testIds } from '../../src/shared/lib/testIds';

const imageUrl = 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==';

const albums = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    title: 'Summer Wedding',
    slug: 'summer-wedding',
    categoryId: 1,
    showInPortfolio: true,
    showInStories: false,
    showInHome: true,
    imageCount: 2,
    coverPublicUrl: imageUrl,
    coverWidth: 1200,
    coverHeight: 800,
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    title: 'Studio Portraits',
    slug: 'studio-portraits',
    categoryId: 2,
    showInPortfolio: true,
    showInStories: true,
    showInHome: false,
    imageCount: 1,
    coverPublicUrl: imageUrl,
    coverWidth: 1200,
    coverHeight: 800,
  },
];

const categories = [
  { id: 1, name: 'Weddings', slug: 'weddings', displayOrder: 1, showAsFilter: true },
  { id: 2, name: 'Portraits', slug: 'portraits', displayOrder: 2, showAsFilter: true },
];

const byTestId = (testId: string): Cypress.Chainable<JQuery<HTMLElement>> => cy.get(`[data-testid="${testId}"]`);

describe('public gallery', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/public/categories', categories).as('categories');
    cy.intercept('GET', '/api/public/albums', albums).as('albums');
    cy.intercept('GET', '/api/public/albums/summer-wedding', {
      ...albums[0],
      description: 'A bright outdoor wedding gallery.',
      images: [
        {
          id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
          albumId: albums[0].id,
          originalName: 'summer-1.jpg',
          storageKey: 'albums/111/images/a.jpg',
          publicUrl: imageUrl,
          width: 1200,
          height: 800,
          orientation: 1,
          imageType: 1,
          sizeBytes: 1024,
        },
      ],
    }).as('albumDetails');
  });

  it('lists public albums and filters by category', () => {
    cy.visit('/portfolio');
    cy.wait(['@categories', '@albums']);

    byTestId(testIds.album.list).within(() => {
      byTestId(testIds.album.card).should('have.length', 2);
      cy.contains('Summer Wedding').should('be.visible');
      cy.contains('Studio Portraits').should('be.visible');
    });

    byTestId(testIds.category.list).contains('Weddings').click();
    byTestId(testIds.album.list).within(() => {
      byTestId(testIds.album.card).should('have.length', 1);
      cy.contains('Summer Wedding').should('be.visible');
      cy.contains('Studio Portraits').should('not.exist');
    });
  });

  it('opens an album detail page from the portfolio grid', () => {
    cy.visit('/portfolio');
    cy.wait(['@categories', '@albums']);

    cy.contains('a', 'Summer Wedding').click();
    cy.wait('@albumDetails');

    cy.url().should('include', '/portfolio/summer-wedding');
    cy.get('h1').contains('Summer Wedding').should('be.visible');
    cy.contains('A bright outdoor wedding gallery.').should('be.visible');
  });
});
