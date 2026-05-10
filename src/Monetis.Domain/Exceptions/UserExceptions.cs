namespace Monetis.Domain.Exceptions;

public class UserPasswordRequiredException() : DomainException("A senha é obrigatória.");

public class UserNewPasswordRequiredException() : DomainException("The new password is required.");

public class UserFirstNameRequiredException() : DomainException("The name is required.");

public class UserFirstNameInvalidException() : DomainException("The first name must contain between 2 and 50 characters.");

public class UserLastNameRequiredException() : DomainException("The last name is required.");

public class UserLastNameInvalidException() : DomainException("The last name must contain between 2 and 50 characters.");

public class UserEmailRequiredException() : DomainException("The email is required.");

public class UserEmailInvalidException() : DomainException("The email format is invalid.");
