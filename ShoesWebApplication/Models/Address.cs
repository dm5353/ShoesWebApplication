using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class Address
{
    public int Id { get; set; }

    public int Index { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int HouseNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
