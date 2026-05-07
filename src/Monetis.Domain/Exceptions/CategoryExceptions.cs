namespace Monetis.Domain.Exceptions;

public class CategoryNameRequiredException() : DomainException("The name of category is required.");

public class CategoryIconRequiredException() : DomainException("The icon is required.");
