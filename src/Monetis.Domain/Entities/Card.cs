namespace Monetis.Domain.Entities;

public class Card : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } 
    public string Name { get; private set; }

    protected Card() { }

    public Card(Guid userId, string name)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("O ID do usuário é obrigatório.");
            
        ValidateName(name);

        UserId = userId;
        Name = name;
    }

    public void Update(string name)
    {
        ValidateName(name);
        Name = name;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The name of card is required.");
    }
}