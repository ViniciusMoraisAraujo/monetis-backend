namespace Monetis.Application.DTOs;

public record CardResponse(Guid Id, string Name, Guid UserId);
public record CreateCardRequest(string Name);
public record UpdateCardRequest(string Name);
