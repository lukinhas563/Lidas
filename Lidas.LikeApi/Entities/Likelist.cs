namespace Lidas.LikeApi.Entities;

public class Likelist
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<Likeitem> Likeitems { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Likelist(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Likeitems = new List<Likeitem>();

        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(Guid userId)
    {
        UserId = userId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;

        UpdatedAt = DateTime.UtcNow;
    }
}
