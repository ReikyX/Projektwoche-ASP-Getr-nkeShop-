// Importiert benötigte Namespaces (Datenbank, Modelle, Identität, usw.)
using DrinkShop.Data;
using DrinkShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Erstellt eine neue Webanwendung mit Konfigurationsoptionen (z.?B. appsettings.json wird geladen)
var builder = WebApplication.CreateBuilder(args);

// -------------------------
// SERVICES REGISTRIEREN
// -------------------------

// Registriert die Datenbankverbindung über Entity Framework Core mit SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registriert ASP.NET Identity mit benutzerdefiniertem Benutzer (ApplicationUser) und Rollenverwaltung
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Für dieses Projekt ist keine E-Mail-Bestätigung beim Login erforderlich
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>() // Identity verwendet denselben DbContext
.AddDefaultTokenProviders(); // Aktiviert Token-Generierung (z.?B. für Passwort-Reset)

// Aktiviert Razor Pages (wird u.?a. für Identity-Seiten benötigt)
builder.Services.AddRazorPages();

// Registriert Session-Verwaltung mit einer Gültigkeit von 30 Minuten
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // Session-Cookies nur per HTTP zugänglich (nicht via JavaScript)
    options.Cookie.IsEssential = true; // Cookie ist essenziell für die Anwendung
});

// Fügt einen benutzerdefinierten Action-Filter hinzu (z.?B. um Artikelanzahl im Warenkorb bereitzustellen)
builder.Services.AddScoped<CartItemCountFilter>();

// Registriert MVC mit Controllern + Views, inkl. dem benutzerdefinierten Filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CartItemCountFilter>();
});


// -------------------------
// MIDDLEWARE-KONFIGURATION
// -------------------------

var app = builder.Build();

// Fehlerbehandlung aktivieren, wenn die App NICHT im Entwicklungsmodus läuft
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); // HTTP Strict Transport Security für mehr Sicherheit (HTTPS erzwingen)
}

// HTTPS-Umleitung aktivieren
app.UseHttpsRedirection();
// Routing aktivieren (für Controller, Razor Pages usw.)
app.UseRouting();

// Identity: Authentifizierung und Autorisierung aktivieren
app.UseAuthentication();
app.UseAuthorization();

// Session aktivieren (z.?B. für Warenkorb-Tracking)
app.UseSession();

// Razor Pages-Routing aktivieren (z.?B. für Identity-Seiten)
app.MapRazorPages();

// Statische Dateien (CSS, JS, Bilder) bereitstellen
app.MapStaticAssets();

// Spezielle Route für die LiveSearch-Methode in ProductsController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "liveSearch",
    pattern: "Products/LiveSearch",
    defaults: new { controller = "Products", action = "LiveSearch" }
);

//await SeedAdminUser(app.Services);

// Startet die Webanwendung
app.Run();

async Task SeedAdminUser(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    const string adminRoleName = "Admin";
    const string adminEmail = "admin@drinkshop.de";
    const string adminPassword = "Admin123!";

    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
        }
        else
        {
            throw new Exception("Fehler beim Erstellen des Admin-Benutzers: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
