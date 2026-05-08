namespace Monetis.Domain.Exceptions;

public class ExpenseAlreadyPaidException() : DomainException("This expense is already paid.");

public class PaidExpenseCannotBeUpdatedException() : DomainException("Cannot update a paid expense");

public class InstallmentExpenseCannotBeUpdatedException() : DomainException("Cannot update individual installment. Update the entire group.");

public class ExpenseCategoryRequiredException() : DomainException("Category is required");

public class ExpenseInstallmentRangeException() : DomainException("Installments must be between 2 and 24");

public class ExpenseTotalAmountMustBePositiveException() : DomainException("Total amount must be greater than zero");

public class ExpenseCreditCardRequiredException() : DomainException("Credit card is required for credit card payments");

public class ExpenseCreditCardRequiredForInstallmentsException() : DomainException("Credit card is required for installments");

public class ExpenseDueDateTooOldException() : DomainException("Due date is too far in the past");

public class ExpenseCreditCardOnlyForCreditCardPaymentException() : DomainException("Credit card should only be set for credit card payments");

public class ExpenseAmountMustBePositiveException() : DomainException("Amount is negative");

public class ExpenseDescriptionInvalidException() : DomainException("Description is invalid");