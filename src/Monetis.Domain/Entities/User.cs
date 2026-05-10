using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    
    protected User () { }
    
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

    private void Validate(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new UserFirstNameRequiredException();
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new UserLastNameRequiredException();
            
        if (string.IsNullOrWhiteSpace(email))
            throw new UserEmailRequiredException();
    }
}
