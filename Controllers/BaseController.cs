using Microsoft.AspNetCore.Mvc;
using DrinkShop.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    protected readonly AppDbContext _context;

    public BaseController(AppDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var itemCount = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            ViewBag.CartItemCount = itemCount;
        }
        else
        {
            ViewBag.CartItemCount = 0;
        }
    }
}
