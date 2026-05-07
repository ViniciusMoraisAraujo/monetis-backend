namespace Monetis.Domain.Exceptions;

public class AccountNameRequiredException() : DomainException("Account name is required");

public class AccountNameTooLongException() : DomainException("Account name must be less than 100 characters");

public class AccountAmountMustBePositiveException() : DomainException("Amount must be greater than zero");

public class AccountAdjustmentReasonInvalidException() : DomainException("Adjustment reason is required (min 5 characters)");
