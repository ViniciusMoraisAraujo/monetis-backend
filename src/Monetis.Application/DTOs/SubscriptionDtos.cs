using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record SubscriptionDto(Guid Id, Guid UserId, Guid AccountId, Guid CategoryId, decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate, bool IsActive);
public record CreateSubscriptionDto(Guid UserId, Guid AccountId, Guid CategoryId, decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate);
public record UpdateSubscriptionDto(decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate, bool IsActive);