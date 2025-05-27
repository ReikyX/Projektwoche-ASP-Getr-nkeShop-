using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrinkShop.Data;
using DrinkShop.Models;

namespace DrinkShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Seed()
        {
            if (!_context.Categories.Any())
            {
                var categories = new List<Category>
            {
                new Category { Id = 1, CategoryName = "Cola" },
                new Category { Id = 2, CategoryName = "Energy" },
                new Category { Id = 3, CategoryName = "Wasser" },
                new Category { Id = 4, CategoryName = "Säfte" },
                new Category { Id = 5, CategoryName = "Eistee & Sonstiges" }
            };
                _context.Categories.AddRange(categories);
            }

            if (!_context.Products.Any())
            {
                var products = new List<Product>
            {
                new Product
                {
                    Name = "Coca-Cola Classic",
                    Description = "Das Original. Erfrischend und belebend.",
                    Price = 1.29m,
                    ImageUrl = "https://cdn.pixabay.com/photo/2021/03/12/17/54/coca-cola-6090176_1280.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Name = "Red Bull",
                    Description = "Energydrink mit belebendem Taurin.",
                    Price = 1.89m,
                    ImageUrl = "https://cdn.pixabay.com/photo/2018/03/14/08/47/red-bull-3222568_1280.jpg",
                    CategoryId = 2
                },

    new Product
    {
        Name = "Pepsi",
        Description = "Cola-Alternative mit einzigartigem Geschmack.",
        Price = 1.19m,
        ImageUrl = "https://cdn.pixabay.com/photo/2015/09/17/17/19/pepsi-942088_1280.jpg",
        CategoryId = 1
    },
    new Product
    {
        Name = "Fanta Orange",
        Description = "Fruchtige Orangenlimonade – süß und spritzig.",
        Price = 1.09m,
        ImageUrl = "https://cdn.pixabay.com/photo/2020/08/10/12/52/fanta-5476817_1280.jpg",
        CategoryId = 1
    },
    new Product
    {
        Name = "Monster Energy",
        Description = "Kräftiger Energydrink für lange Nächte.",
        Price = 2.19m,
        ImageUrl = "https://cdn.pixabay.com/photo/2020/04/11/16/20/monster-energy-5030473_1280.jpg",
        CategoryId = 2
    },
    new Product
    {
        Name = "Vio Stillwasser",
        Description = "Natürliches Mineralwasser ohne Kohlensäure.",
        Price = 0.89m,
        ImageUrl = "https://cdn.pixabay.com/photo/2016/07/12/16/55/water-1510929_1280.jpg",
        CategoryId = 3
    },
    new Product
    {
        Name = "Apfelsaft Klar",
        Description = "Klarer Apfelsaft aus sonnengereiften Äpfeln.",
        Price = 1.49m,
        ImageUrl = "https://cdn.pixabay.com/photo/2015/09/02/12/41/apple-918408_1280.jpg",
        CategoryId = 4
    },
    new Product
    {
        Name = "Eistee Pfirsich",
        Description = "Kalter Schwarztee mit fruchtigem Pfirsichgeschmack.",
        Price = 1.29m,
        ImageUrl = "https://cdn.pixabay.com/photo/2021/04/19/10/35/iced-tea-6189599_1280.jpg",
        CategoryId = 5
    },
    new Product
    {
        Name = "Capri-Sun Orange",
        Description = "Der Klassiker in der Trinkpackung.",
        Price = 0.99m,
        ImageUrl = "https://cdn.pixabay.com/photo/2020/08/27/15/11/juice-5522217_1280.jpg",
        CategoryId = 5
    }
            };

                _context.Products.AddRange(products);
            }

            await _context.SaveChangesAsync();

            return Content("Seed-Daten eingefügt");
        }



        // GET: Products
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Products.Include(p => p.Category);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageUrl,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
