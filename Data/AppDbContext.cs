// Importiert die Models (z.B. ApplicationUser, Category, Product, CartItem)
using DrinkShop.Models;

// Basis-Klasse für Identity mit Entity Framework Core (für Benutzerverwaltung)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// Entity Framework Core - das ORM für den Datenbankzugriff
using Microsoft.EntityFrameworkCore;

namespace DrinkShop.Data;

// Definiert den Datenbank-Kontext für die Anwendung,
// erbt von IdentityDbContext mit dem Benutzer-Typ ApplicationUser
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    // Konstruktor mit Optionen für die Datenbankverbindung
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet für die Kategorien-Tabelle in der Datenbank
    public DbSet<Category> Categories { get; set; }

    // DbSet für die Produkte-Tabelle in der Datenbank
    public DbSet<Product> Products { get; set; }

    // DbSet für die Warenkorb-Elemente in der Datenbank
    public DbSet<CartItem> CartItems { get; set; }
}
