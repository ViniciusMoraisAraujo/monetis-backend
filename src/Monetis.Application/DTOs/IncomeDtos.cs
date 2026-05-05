namespace Monetis.Application.DTOs;

public record IncomeResponse(
    Guid Id, 
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);

public record CreateIncomeRequest(
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);

public record UpdateIncomeRequest(
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);
