using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrinkShop.Data;
using DrinkShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace DrinkShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        // Konstruktor: Der DbContext wird per Dependency Injection übergeben
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Products
        // Anzeige aller Produkte (optional gefiltert nach Kategorie)
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Übergibt die Kategorienliste an die View für das Dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;

            // Holt alle Produkte inkl. zugehöriger Kategorie
            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            // Wenn eine Kategorie ausgewählt wurde, wird gefiltert
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            return View(await products.ToListAsync());
        }



        // GET: Products/Details/5
        // Anzeige der Detailansicht eines Produkts
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Kein ID übergeben
            }
            // Sucht Produkt inkl. Kategorie
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound(); // Produkt nicht gefunden
            }

            return View(product);
        }

        // GET: Products/Create
        // Anzeige des Formulars zur Erstellung eines neuen Produkts
        public IActionResult Create()
        {
            // Erstellt Dropdownliste der Kategorien
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // Speichert ein neues Produkt
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageUrl,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Holt die Benutzer-ID des aktuell angemeldeten Users
                product.UserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(product.UserId))
                {
                    // Benutzer nicht angemeldet → Fehlermeldung
                    ModelState.AddModelError(string.Empty, "User ist nicht angemeldet.");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
                    return View(product);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Falls Validierung fehlschlägt → View erneut anzeigen mit eingegebenen Werten
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }


        // GET: Products/Edit/5
        // Anzeige des Formulars zum Bearbeiten eines Produkts
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product?.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // Speichert die Änderungen an einem Produkt
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound(); // ID stimmt nicht überein
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound(); // Produkt existiert nicht mehr
                    }
                    else
                    {
                        throw; // Fehler weiterreichen
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Falls Validierung fehlschlägt → View erneut anzeigen
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        // Anzeige der Bestätigungsseite für das Löschen
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        // Führt das Löschen eines Produkts aus
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Hilfsmethode: Prüft, ob ein Produkt existiert
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpGet]
        [AllowAnonymous]
        // AJAX-basierte Livesuche (anonym erlaubt)
        public IActionResult LiveSearch(string query)
        {
            var userIsAdmin = User.IsInRole("Admin");
            var userIsAuthenticated = User.Identity.IsAuthenticated;

            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<object>());
            }

            var loweredQuery = query.ToLower();

            // Sucht nach Produkten, deren Namen die Abfrage enthalten
            var results = _context.Products
                .Where(p => p.Name.ToLower().Contains(loweredQuery))
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    imageUrl = p.ImageUrl,
                    categoryName = p.Category.CategoryName,
                    isAdmin = userIsAdmin,
                    isAuthenticated = userIsAuthenticated
                })
                .ToList();

            return Json(results);
        }




    }
}
