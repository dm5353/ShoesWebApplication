using System;
using System.Collections.Generic;

namespace ShoesWebApplication.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly SupplyDate { get; set; }

    public int AddressId { get; set; }

    public int Code { get; set; }

    public int OrderStatusId { get; set; }

    public int UserId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
