using BuildingBlocks.Common.BaseClasses;

namespace Products.Domain.Entities;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public ICollection<Product> Products { get; private set; }

    private Category()
    {
        Products = new List<Product>();
    }

    public Category(string name, string description, Guid? parentCategoryId = null) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        ParentCategoryId = parentCategoryId;
    }

    public void Update(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
    }
}
