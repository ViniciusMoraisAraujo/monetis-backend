using Monetis.Domain.Enums;

namespace Monetis.Application.DTOs;

public record SubscriptionResponse(
    Guid Id,
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    bool IsActive
);

public record CreateSubscriptionRequest(
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate
);

public record UpdateSubscriptionRequest(
    decimal Amount,
    string Description,
    Frequency Frequency,
    DateTime NextDueDate,
    bool IsActive
);
