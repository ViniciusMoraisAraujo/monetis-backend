namespace Monetis.Application.DTOs;

public record CardDto(Guid Id, string Name, Guid userId );
public record CreateCardDto(string Name);
public record UpdateCardDto(string Name);
