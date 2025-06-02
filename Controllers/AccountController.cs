// Models für Benutzer und ViewModels (RegisterViewModel, LoginViewModel)
using DrinkShop.Models;

// ASP.NET Core Identity – enthält Funktionen zur Benutzerverwaltung
using Microsoft.AspNetCore.Identity;

// MVC-Controller-Funktionalitäten
using Microsoft.AspNetCore.Mvc;

namespace DrinkShop.Controllers
{
    // Dieser Controller ist zuständig für Login, Logout & Registrierung
    public class AccountController : Controller
    {
        // Identity-Komponenten:
        private readonly UserManager<ApplicationUser> _userManager;         // Verwaltet Benutzer (Erstellen, Löschen, Abfragen etc.)
        private readonly SignInManager<ApplicationUser> _signInManager;     // Verwalten von Login / Logout

        // Konstruktor mit Dependency Injection
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ---------- Registrierung ----------

        // Zeigt das Registrierungsformular an (GET)
        [HttpGet]
        public IActionResult Register() => View();

        // Verarbeitet die Registrierung (POST)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Wenn das Formular ungültig ist → zurück zur View mit Validierungsfehlern
            if (!ModelState.IsValid) return View(model);

            // Neues Benutzerobjekt mit E-Mail als Benutzername
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            // Benutzer anlegen (inkl. Passwort)
            var result = await _userManager.CreateAsync(user, model.Password);

            // Wenn die Registrierung erfolgreich war
            if (result.Succeeded)
            {
                // Benutzer einloggen (nicht persistent = bleibt nicht nach Schließen des Browsers angemeldet)
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Weiterleitung auf die Startseite (Produkte)
                return RedirectToAction("Index", "Products");
            }

            // Wenn Fehler auftraten (z. B. schwaches Passwort, Benutzer existiert bereits)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description); // Fehler in die ModelState aufnehmen
            }

            return View(); // Erneut das Formular anzeigen mit Fehlern
        }

        // ---------- Login ----------

        // Zeigt das Loginformular an (GET)
        [HttpGet]
        public IActionResult Login() => View();

        // Verarbeitet die Login-Daten (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Ungültiges Formular → zurück zur View
            if (!ModelState.IsValid) return View(model);

            // Anmeldung mit E-Mail und Passwort, nicht dauerhaft (kein "angemeldet bleiben"), kein Logout bei Fehlschlägen
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            // Login erfolgreich → Weiterleitung auf Produkte-Seite
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Products");
            }

            // Bei Fehlschlag → Fehlermeldung anzeigen
            ModelState.AddModelError("", "E-Mail oder Passwort ist ungültig.");
            return View(model); // Eingaben beibehalten
        }

        // ---------- Logout ----------

        // Meldet den Benutzer ab (POST)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Benutzer abmelden
            await _signInManager.SignOutAsync();

            // Session-Variablen löschen (z. B. Warenkorb)
            HttpContext.Session.Clear();

            // Zurück zur Startseite
            return RedirectToAction("Index", "Products");
        }
    }
}
