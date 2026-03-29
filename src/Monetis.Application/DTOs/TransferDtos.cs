namespace Monetis.Application.DTOs;

public record TransferDto(
    Guid Id, 
    Guid AccountId, 
    Guid DestinationAccountId, 
    decimal Amount, 
    string Description, 
    DateTime TransferredAt);

public record CreateTransferDto(
    Guid AccountId, 
    Guid DestinationAccountId, 
    decimal Amount, 
    string Description, 
    DateTime TransferredAt);

public record UpdateTransferDto(
    decimal Amount, 
    string Description);