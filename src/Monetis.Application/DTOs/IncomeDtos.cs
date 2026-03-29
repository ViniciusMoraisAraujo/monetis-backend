namespace Monetis.Application.DTOs;

public record IncomeDto(
    Guid Id, 
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);

public record CreateIncomeDto(
    Guid AccountId, 
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);

public record UpdateIncomeDto(
    Guid CategoryId, 
    decimal Amount, 
    string Description, 
    DateTime ReceivedAt);