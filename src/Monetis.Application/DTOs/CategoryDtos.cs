using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record CategoryDto(Guid Id, string Name, Guid UserId, TransactionType Type, string Icon);
public record CreateCategoryDto(string Name, Guid UserId, TransactionType Type, string Icon);
public record UpdateCategoryDto(string Name, string Icon);