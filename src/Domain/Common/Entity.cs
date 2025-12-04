namespace Domain.Common;

public abstract class Entity
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; } 

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
}