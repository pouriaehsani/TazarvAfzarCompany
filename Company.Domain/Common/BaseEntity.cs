namespace Company.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsDeleted { get; set; }
}