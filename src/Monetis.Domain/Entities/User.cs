using System.Text.RegularExpressions;

using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class User : BaseEntity
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName => $"{FirstName} {LastName}";

    protected User()
    {
        //required for ef
    }

    public User(string firstName, string lastName, string email, string passwordHash)
    {
        Validate(firstName, lastName, email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new UserPasswordRequiredException();

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public void Update(string firstName, string lastName, string email)
    {
        Validate(firstName, lastName, email);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new UserNewPasswordRequiredException();

        PasswordHash = newPasswordHash;
    }
    private static void Validate(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new UserFirstNameRequiredException();

        if (firstName.Length > 50 || firstName.Length < 2)
            throw new UserFirstNameInvalidException();

        if (string.IsNullOrWhiteSpace(lastName))
            throw new UserLastNameRequiredException();

        if (lastName.Length > 50 || lastName.Length < 2)
            throw new UserLastNameInvalidException();

        if (string.IsNullOrWhiteSpace(email))
            throw new UserEmailRequiredException();

        if (!EmailRegex.IsMatch(email.Trim()))
            throw new UserEmailInvalidException();
    }
}
