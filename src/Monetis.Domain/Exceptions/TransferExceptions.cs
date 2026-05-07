namespace Monetis.Domain.Exceptions;

public class TransferOriginAccountRequiredException() : DomainException("Origin account is required");

public class TransferDestinationAccountRequiredException() : DomainException("Destination account is required");

public class TransferAccountsMustBeDifferentException() : DomainException("Origin and destination accounts must be different");

public class TransferAccountsMustBelongToSameUserException() : DomainException("Both accounts must belong to the same user");

public class TransferAmountMustBePositiveException() : DomainException("Transfer amount must be greater than zero");

public class TransferDateInFutureException() : DomainException("Transfer date cannot be in the future");

public class TransferInsufficientFundsException(decimal balance, decimal amount)
    : DomainException(
        $"Insufficient funds for transfer. Current balance: {balance:C}, Transfer amount: {amount:C}, Missing: {amount - balance:C}");

public class TransferAlreadyCancelledException() : DomainException("Transfer is already cancelled");

public class TransferCancellationSameDayOnlyException() : DomainException("Transfer can only be cancelled on the same day");

public class TransferOriginAccountMismatchException() : DomainException("Origin account does not match the transfer's origin account");
