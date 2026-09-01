using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public User? User { get; init; }
    public string Icon { get; private set; } = null!;

    protected Category()
    {
        //required for ef
    }

    public Category(string name, Guid userId, string icon)
    {
        ValidateCategory(name, icon);
        Name = name;
        UserId = userId;
        Icon = icon;
    }

    public static Category CreateSystemCategory(Guid id, string name, string icon)
    {
        return new Category
        {
            Id = id,
            CreatedAt = new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc),
            Name = name,
            Icon = icon
        };
    }

    public void Update(string name, string icon)
    {
        Name = name;
        Icon = icon;
    }

    private static void ValidateCategory(string name, string icon)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CategoryNameRequiredException();

        if (string.IsNullOrWhiteSpace(icon))
            throw new CategoryIconRequiredException();
    }
}
