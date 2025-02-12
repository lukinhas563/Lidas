﻿namespace Lidas.UserApi.Models.View;

public class RoleView
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public List<UserViewList> Users { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
