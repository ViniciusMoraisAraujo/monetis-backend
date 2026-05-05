namespace Monetis.Application.DTOs;

public record TransferResponse(
    Guid Id, 
    Guid AccountId, 
    Guid DestinationAccountId, 
    decimal Amount, 
    string Description, 
    DateTime TransferredAt);

public record CreateTransferRequest(
    Guid AccountId, 
    Guid DestinationAccountId, 
    decimal Amount, 
    string Description, 
    DateTime TransferredAt);

public record UpdateTransferRequest(
    decimal Amount, 
    string Description );
