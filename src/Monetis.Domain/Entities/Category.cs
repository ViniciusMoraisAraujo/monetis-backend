namespace Monetis.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public Guid? UserId { get; private set; }
    public User? User { get; private set; }
    public string Icon { get; private set; }
    
    protected Category() { }
    
    public Category(string name, Guid userId,  string icon)
    {
        ValidateCategory(name, icon);
        Name = name;
        UserId = userId;
        Icon = icon;
    }

    public static Category CreateSystemCategory(Guid id, string name,  string icon)
    {
        return new Category
        {
            Id = id,
            CreatedAt = new DateTime(2026, 3, 13),
            Name = name,
            Icon = icon
        };
    }

    public void Update(string name, string icon)
    {
        Name = name;
        Icon = icon;
    }

    private void ValidateCategory(string name, string icon)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The name of category is required.");
            
        if (string.IsNullOrWhiteSpace(icon))
            throw new ArgumentException("The icon is required.");
    }
}