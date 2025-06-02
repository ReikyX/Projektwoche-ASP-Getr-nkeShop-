// Diese Namespaces werden benötigt:
using Microsoft.AspNetCore.Mvc;               // Für Controller-Funktionalität
using DrinkShop.Data;                         // Zugriff auf den AppDbContext (Entity Framework)
using System.Security.Claims;                 // Zugriff auf Benutzerinformationen (Claims)
using System.Threading.Tasks;
using System.Linq;                            // LINQ-Methoden (z. B. Where, Sum)
using Microsoft.AspNetCore.Mvc.Filters;       // Für Action-Filter (z. B. OnActionExecuting)


// Der BaseController erbt von "Controller" und kann von anderen Controllern abgeleitet werden
public class BaseController : Controller
{
    // Zugriff auf die Datenbank (Dependency Injection des AppDbContext)
    protected readonly AppDbContext _context;

    // Konstruktor: Der Datenbankkontext wird injiziert und gespeichert
    public BaseController(AppDbContext context)
    {
        _context = context;
    }

    // Diese Methode wird VOR jeder Action ausgeführt (wenn ein Controller von BaseController erbt)
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Basisfunktionalität aufrufen
        base.OnActionExecuting(context);

        // Prüfen, ob der Benutzer angemeldet ist
        if (User.Identity.IsAuthenticated)
        {
            // Benutzer-ID aus dem aktuellen Benutzerkontext (Claims) auslesen
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Anzahl der Artikel im Warenkorb dieses Benutzers berechnen (Summe der Mengen)
            var itemCount = _context.CartItems
                .Where(c => c.UserId == userId)     // Nur Warenkorb des aktuellen Users
                .Sum(c => c.Quantity);              // Gesamtanzahl aller Produkte

            // Ergebnis im ViewBag speichern → steht später in der View zur Verfügung (z. B. Navbar)
            ViewBag.CartItemCount = itemCount;
        }
        else
        {
            // Wenn nicht eingeloggt: Artikelanzahl = 0
            ViewBag.CartItemCount = 0;
        }
    }
}
