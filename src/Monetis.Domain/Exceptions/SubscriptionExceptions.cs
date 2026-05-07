using Monetis.Domain.Enums;

namespace Monetis.Domain.Exceptions;

public class SubscriptionInactiveProcessException() : DomainException("Cannot process inactive subscription");

public class SubscriptionEndedException() : DomainException("Subscription has reached end date");

public class SubscriptionNotDueYetException(DateTime nextDueDate)
    : DomainException($"Subscription is not due yet. Next due: {nextDueDate:dd/MM/yyyy}");

public class SubscriptionCannotReactivateAfterEndDateException() : DomainException("Cannot reactivate subscription after end date");

public class SubscriptionInvalidFrequencyException() : DomainException("Invalid frequency");

public class SubscriptionCategoryRequiredException() : DomainException("Category is required");

public class SubscriptionAmountMustBePositiveException() : DomainException("Amount must be greater than zero");

public class SubscriptionDescriptionInvalidException() : DomainException("Description is required and must be less than 200 characters");

public class SubscriptionDescriptionRequiredException() : DomainException("Description is required");

public class SubscriptionCardRequiredException() : DomainException("Card is required for credit card payments");

public class SubscriptionAccountRequiredException(PaymentMethod paymentMethod)
    : DomainException($"Account is required for {paymentMethod} payments");

public class SubscriptionCardOnlyForCreditCardPaymentException() : DomainException("Card should only be specified for credit card payments");
