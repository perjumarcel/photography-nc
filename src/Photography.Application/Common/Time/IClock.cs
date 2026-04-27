namespace Photography.Application.Common.Time;

/// <summary>Abstraction over <see cref="DateTime.UtcNow"/> to enable testable time-dependent logic.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
