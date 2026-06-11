using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class Order
{
    [ValidateNever]
    public int Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly SupplyDate { get; set; }

    public int AddressId { get; set; }

    public int Code { get; set; }

    public int OrderStatusId { get; set; }

    public int UserId { get; set; }
    [ValidateNever]
    public virtual Address Address { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    [ValidateNever]
    public virtual OrderStatus OrderStatus { get; set; } = null!;
    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
