namespace Monetis.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    public User(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
}