using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();
}
