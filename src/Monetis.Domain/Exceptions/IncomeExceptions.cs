namespace Monetis.Domain.Exceptions;

public class IncomeReceivedDateInFutureException() : DomainException("Received date cannot be in the future for paid incomes");

public class IncomeExpectedDateMustBeFutureException() : DomainException("Expected date must be in the future for scheduled incomes");

public class IncomeAlreadyReceivedException() : DomainException("Income is already received");

public class ReceivedIncomeCannotBeCancelledException() : DomainException("Cannot cancel an already received income");

public class IncomeAmountMustBePositiveException() : DomainException("Income amount must be greater than zero");

public class IncomeCategoryRequiredException() : DomainException("Category is required");
