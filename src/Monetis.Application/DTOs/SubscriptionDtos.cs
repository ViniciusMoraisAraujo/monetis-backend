using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record SubscriptionDto(Guid Id,   decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate, bool IsActive);
public record CreateSubscriptionDto(decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate);
public record UpdateSubscriptionDto(decimal Amount, string Description, Frequency Frequency, DateTime NextDueDate, bool IsActive);