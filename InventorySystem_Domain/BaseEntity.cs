namespace InventorySystem_Domain;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    public DateTime ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }

    public virtual void SetCreated(int createdBy)
    {
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    public virtual void UpdateMetadata(int modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}

