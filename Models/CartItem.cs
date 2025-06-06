﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DrinkShop.Models;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }

    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}
