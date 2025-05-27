using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DrinkShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Bitte geben Sie einen Namen ein.")]
    public string Name { get; set; } 
    public string Description { get; set; }

    [Range(0.1, 1000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }

    [Required(ErrorMessage = "Bitte wählen Sie eine Kategorie.")]
    [Range(1, int.MaxValue, ErrorMessage = "Bitte wählen Sie eine gültige Kategorie.")]
    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
