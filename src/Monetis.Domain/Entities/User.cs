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
            throw new ArgumentException("A senha é obrigatória.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public void Update(string firstName, string lastName, string email)
    {
        Validate(firstName, lastName, email);
        
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("The new password is required.");
            
        PasswordHash = newPasswordHash;
    }

    private void Validate(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("The name is required.");
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("The last name is required.");
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("The email is required.");
    }
}