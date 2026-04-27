# Skill: Backend Result&lt;T&gt; Pattern

> Return Result&lt;T&gt; from services/handlers for known failures — never throw for domain validation.

## When to Apply

- Returning success or failure from Application services or MediatR handlers
- Mapping results to HTTP responses in controllers

## Usage

```csharp
// Success
return Result<Guid>.Ok(booking.BookingId);

// Failure — known domain error
return Result<Guid>.Fail("Time slot is not available");

// Never throw exceptions for domain validation
// Exceptions are for truly unexpected errors only
```

## Controller Mapping

```csharp
if (!result.Success)
{
    return result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? NotFound(new { message = result.Error })
        : BadRequest(new { message = result.Error });
}
return Ok(result.Data);
```

## Error Categories

| Error Type | Return | HTTP |
|-----------|--------|------|
| Not found | `Result.Fail("Entity not found")` | 404 |
| Validation | `Result.Fail("Invalid input: ...")` | 400 |
| Conflict | `Result.Fail("Already exists")` | 409 |
| Business rule | `Result.Fail("Cannot transition...")` | 400 |
| Unauthorized | Handled by middleware/filters | 401/403 |
| Unexpected | Exception → GlobalExceptionHandler → ProblemDetails | 500 |

## Rules

- **Always** return `Result.Fail()` for known errors — never throw.
- Exceptions only for truly unexpected errors (caught by `GlobalExceptionHandlerMiddleware`).
- Uncaught exceptions → ProblemDetails (RFC 7807) with `correlationId` and `traceId`.

## Checklist

- [ ] Handlers/services return `Result<T>` — not raw types
- [ ] `Result.Fail()` for domain errors — no exceptions
- [ ] Controllers map Result to appropriate HTTP status
- [ ] Correlation IDs included in error responses
