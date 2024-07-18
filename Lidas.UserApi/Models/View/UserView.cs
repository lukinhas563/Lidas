namespace Lidas.UserApi.Models.View;

public class UserView
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<RoleView> Role { get; set; }

    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
