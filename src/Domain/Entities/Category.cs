using Domain.Common;


namespace Domain.Entities;

public class Category : Entity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public override string ToString()
    {
        return $"Category {{ Id = {Id}, Name = \"{Name}\" }}";
    }
}