using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using DrinkShop.Data;

public class CartItemCountFilter : IActionFilter
{
    private readonly AppDbContext _context;

    public CartItemCountFilter(AppDbContext context)
    {
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        var user = context.HttpContext.User;

        if (controller != null && user.Identity.IsAuthenticated)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var itemCount = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            controller.ViewBag.CartItemCount = itemCount;
        }
        else if (controller != null)
        {
            controller.ViewBag.CartItemCount = 0;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
