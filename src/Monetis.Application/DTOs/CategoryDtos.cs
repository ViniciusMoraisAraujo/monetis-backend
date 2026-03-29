using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record CategoryDto(Guid Id, string Name, Guid UserId,  string Icon);
public record CreateCategoryDto(string Name, string Icon);
public record UpdateCategoryDto(string Name, string Icon);