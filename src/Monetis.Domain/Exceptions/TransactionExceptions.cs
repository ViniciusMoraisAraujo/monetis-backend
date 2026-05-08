namespace Monetis.Domain.Exceptions;

public class TransactionInvalidAccountException() : DomainException("Invalid Account Id");

public class TransactionAmountMustBePositiveException() : DomainException("Transaction amount must be greater than zero");

public class TransactionDescriptionRequiredException() : DomainException("Transaction description is required");

public class TransactionDescriptionTooLongException() : DomainException("Transaction description must be less than or equal to 200 characters");
