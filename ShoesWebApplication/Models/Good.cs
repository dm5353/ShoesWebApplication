using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoesWebApplication.Models;

public partial class Good
{
    [ValidateNever]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Скидка не может быть отрицательным")]
    public byte Discount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
    public int Count { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }

    public int GoodMeasurementId { get; set; }

    public int GoodCategoryId { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    [ValidateNever]
    public virtual GoodCategory GoodCategory { get; set; } = null!;
    [ValidateNever]
    public virtual Measurement GoodMeasurement { get; set; } = null!;
    [ValidateNever]
    public virtual Manufacturer Manufacturer { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    [ValidateNever]
    public virtual Supplier Supplier { get; set; } = null!;
}
