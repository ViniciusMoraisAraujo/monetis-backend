using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class Card : UserOwnedEntity
{
    public string Name { get; private set; } = null!;

    protected Card()
    {
        //required for EF
    }


    public Card(string name)
    {
        ValidateName(name);

        Name = name;
    }

    public void Update(string name)
    {
        ValidateName(name);
        Name = name;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CardNameRequiredException();
    }
}
