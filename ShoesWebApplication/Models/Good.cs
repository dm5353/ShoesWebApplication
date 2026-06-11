using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class Good
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public byte Discount { get; set; }

    public int Count { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    public int GoodMeasurementId { get; set; }

    public int GoodCategoryId { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public virtual GoodCategory GoodCategory { get; set; } = null!;

    public virtual Measurement GoodMeasurement { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;
}
