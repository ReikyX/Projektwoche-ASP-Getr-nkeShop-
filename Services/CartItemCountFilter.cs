using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using DrinkShop.Data;

public class CartItemCountFilter : IActionFilter
{
    private readonly AppDbContext _context;

    // Konstruktor: DbContext wird per Dependency Injection übergeben
    public CartItemCountFilter(AppDbContext context)
    {
        _context = context;
    }

    // Wird **vor** der Ausführung einer Controller-Action ausgeführt
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Versuche, den Controller aus dem Kontext zu holen und User-Informationen zu bekommen
        var controller = context.Controller as Controller;
        var user = context.HttpContext.User;

        // Falls Controller existiert und User angemeldet ist
        if (controller != null && user.Identity.IsAuthenticated)
        {
            // User-ID aus den Claims auslesen
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            // Alle CartItems für diesen User summieren (Summe der Mengen)
            var itemCount = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            // Anzahl der Artikel im Warenkorb in ViewBag speichern (für View zugänglich)
            controller.ViewBag.CartItemCount = itemCount;
        }
        else if (controller != null) // User nicht angemeldet: Warenkorb auf 0 setzen
        {
            controller.ViewBag.CartItemCount = 0;
        }
    }

    // Wird **nach** der Ausführung einer Controller-Action ausgeführt
    // Hier leer, da keine Nachbearbeitung notwendig
    public void OnActionExecuted(ActionExecutedContext context) { }
}
