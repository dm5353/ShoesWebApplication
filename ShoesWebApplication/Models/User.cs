using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class User
{
    public int Id { get; set; }

    public int UserRoleId { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual UserRole UserRole { get; set; } = null!;
}
