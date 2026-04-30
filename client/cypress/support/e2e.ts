declare global {
  namespace Cypress {
    interface Chainable {
      byTestId(testId: string): Cypress.Chainable<JQuery<HTMLElement>>;
    }
  }
}

Cypress.Commands.add('byTestId', (testId: string) => cy.get(`[data-testid="${testId}"]`));

export {};
