namespace Monetis.Domain.Exceptions;

public class UserOwnedEntityUserAlreadySetException() : DomainException("UserId já foi definido e não pode ser alterado.");
