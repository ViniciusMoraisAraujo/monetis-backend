namespace Monetis.Domain.Exceptions;

public class UserPasswordRequiredException() : DomainException("A senha é obrigatória.");

public class UserNewPasswordRequiredException() : DomainException("The new password is required.");

public class UserFirstNameRequiredException() : DomainException("The name is required.");

public class UserLastNameRequiredException() : DomainException("The last name is required.");

public class UserEmailRequiredException() : DomainException("The email is required.");
