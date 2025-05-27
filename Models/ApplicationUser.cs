using Microsoft.AspNetCore.Identity;

namespace DrinkShop.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<CartItem> CartItems { get; set; }
}
