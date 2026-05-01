namespace Photography.Application.Contact.Dtos;

public sealed record ContactMessageDto(
    string? Name,
    string? Email,
    string? Message,
    string? Phone = null,
    string? EventType = null,
    string? PreferredDate = null,
    string? Venue = null,
    string? EstimatedBudgetRange = null,
    string? SourcePage = null,
    string? Website = null);
