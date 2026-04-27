# Skill: Backend Testing (xUnit + Moq)

> Arrange-Act-Assert with Moq, testing services, entities, and handlers.

## When to Apply

- Writing tests for domain entities, services, or handlers
- Verifying business logic, status transitions, or validation

## Test Location

```
tests/
  ReflectStudio.Core.Tests/           ← Entity, value object tests
  ReflectStudio.Application.Tests/    ← Service, handler, event handler tests
  ReflectStudio.Infrastructure.Tests/ ← Repository, EF Core tests
  ReflectStudio.Web.Tests/            ← Controller, middleware tests
```

## Naming Convention

```csharp
// Pattern: MethodName_Condition_ExpectedResult
[Fact]
public async Task CreateBooking_ReturnsOk_WhenSlotAvailable()
public async Task CreateBooking_ReturnsFail_WhenSlotOverlaps()
public void CanTransitionTo_ReturnsTrue_WhenPendingToConfirmed()
public void Constructor_ThrowsArgumentException_WhenStudioIdEmpty()
```

## Service Test Template

```csharp
public class BookingServiceTests
{
    private readonly Mock<IBookingCommandRepository> _commandRepo = new();
    private readonly Mock<IBookingQueryRepository> _queryRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _sut = new BookingService(_commandRepo.Object, _queryRepo.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateBooking_ReturnsOk_WhenSlotAvailable()
    {
        // Arrange
        _queryRepo.Setup(r => r.AnyOverlappingAsync(
                It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateBookingAsync(dto, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _commandRepo.Verify(r => r.Add(It.IsAny<Booking>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## Rules

- **Arrange-Act-Assert** pattern in every test.
- `Moq` for repository/service interfaces.
- `CancellationToken.None` in test calls.
- Verify calls with `Times.Once` / `Times.Never`.
- Test **both** success and failure paths.
- Test status machine transitions (valid + invalid).
- Run: `cd tests && dotnet test`

## Checklist

- [ ] Naming: `MethodName_Condition_ExpectedResult`
- [ ] Arrange-Act-Assert pattern
- [ ] Moq for interfaces, `CancellationToken.None` in calls
- [ ] Both success and failure paths covered
- [ ] Status transitions tested (valid + invalid)
