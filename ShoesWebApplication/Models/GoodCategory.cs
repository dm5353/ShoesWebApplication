using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class GoodCategory
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();
}
