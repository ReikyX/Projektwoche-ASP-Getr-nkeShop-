using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DrinkShop.Models;

public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; }

    [ValidateNever]
    public ICollection<Product> Products { get; set; }

}
