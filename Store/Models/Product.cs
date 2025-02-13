using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string NameProduct { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly? DateUp { get; set; }

    public bool Discontinued { get; set; }

    public string? PhotoUrl { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}
