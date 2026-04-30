namespace Photography.Application.Contact.Dtos;

public sealed record ContactMessageDto(string? Name, string? Email, string? Message, string? Website = null);
