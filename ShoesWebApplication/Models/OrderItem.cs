using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string GoodId { get; set; } = null!;

    public int Count { get; set; }

    public virtual Good Good { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
