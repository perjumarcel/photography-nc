# Skill: Fix Pre-Existing Issues Identified by Code Review

## When to Apply

Apply this policy whenever:

- The **code review tool** identifies issues in any file, even if the file was **not modified** in the current merge request changeset.
- A code reviewer (human or automated) flags a bug, missing fallback, misleading comment, type safety issue, or other defect in existing code.

## The Rule

> **Code review findings must be fixed regardless of whether the affected code was part of the current changeset.**
>
> If the reviewer identifies a real issue — fix it. Do not dismiss it as "pre-existing" or "out of scope."

## Rationale

1. **Code quality is cumulative** — Ignoring known issues creates technical debt that compounds over time.
2. **Reviewer trust** — If the reviewer flags something and it's dismissed, future review feedback may be ignored too.
3. **Opportunity cost** — The issue is already identified and understood. Fixing it now is cheaper than rediscovering it later.
4. **Safety** — Pre-existing bugs in related code (e.g., missing error fallbacks in auth) can cause runtime failures that affect the feature being changed.

## How to Apply

### Step 1: Evaluate the finding

Determine if the code review comment identifies a **real issue**:
- Missing error handling / fallbacks
- Type safety gaps (e.g., `as string` without fallback)
- Misleading comments
- Dead code
- Security concerns

### Step 2: Fix it in the same PR

Make the fix in the current branch, even if the file wasn't otherwise modified. Include:
- The code fix
- Tests for the new behavior (if applicable)
- A note in the PR description that the fix addresses a pre-existing issue found during review

### Step 3: If the fix is large, create a follow-up issue

If the pre-existing issue requires a significant refactor that would bloat the current PR:
- Create a GitHub issue describing the problem
- Reference it in the PR description
- Fix it in a separate PR promptly

## Example

**Code review finds:** `state.error = action.payload as string` without fallback in a Redux slice rejected handler.

**Even though** the slice wasn't modified in the current PR, the fix is:
```typescript
// Before (unsafe — payload can be undefined if rejectWithValue is not called)
state.error = action.payload as string;

// After (safe — provides meaningful fallback)
state.error = (action.payload as string) || 'Operation failed';
```

Add a test to verify:
```typescript
it('should handle rejected with undefined payload', () => {
  const state = reducer(loadingState, { type: thunk.rejected.type, payload: undefined });
  expect(state.error).toBe('Operation failed');
});
```

## Reference

- This policy was established after code review of PR #156 identified pre-existing issues in `authSlice.ts` and `Program.cs` that were not part of the changeset but affected code quality.
