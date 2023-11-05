using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategories.Models;

namespace ProductCategories.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

        //This root route (ie. Index method) is simply here to redirect the user to the products route
        [HttpGet("")]
        public IActionResult Index()
        {
            return Redirect("products");
        }

//////////////////////////////PRODUCT METHODS\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("products")]
        public IActionResult Products()
        {
            List<Product> EveryProduct = _context.Products.ToList();
            ViewBag.AllProducts = EveryProduct;
            return View("Products");
        }

        [HttpPost("InsertProduct")]
        public IActionResult InsertProduct(Product newProd)
        {
            _context.Add(newProd);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpGet("products/{prodId}")]
        public IActionResult SpecificProduct(int prodId)
        {
            Product getProd = _context.Products.FirstOrDefault(p => p.ProductId == prodId);
            ViewBag.ThisProduct = getProd;

            var prodWithCats = _context.Products
                .Include(p => p.AssocCats)
                .ThenInclude(c => c.Category)
                .FirstOrDefault(p => p.ProductId == prodId);
            
            ViewBag.ProductWithCategories = prodWithCats;

            List<Category> EveryCategory = _context.Categories.ToList();
            List<Category> SomeCategories = new List<Category>();

            foreach (var c in prodWithCats.AssocCats)
            {
                SomeCategories.Add(c.Category);
            }
            List<Category> NotYetAssoc = EveryCategory.Except(SomeCategories).ToList();
            ViewBag.NotYetAssoc = NotYetAssoc;
            return View("SpecificProduct");
        }

        [HttpPost("AddCatToProd")]
        public IActionResult AddCatToProd(Association newAssoc)
        {
            _context.Add(newAssoc);
            _context.SaveChanges();
            return Redirect("/products/"+newAssoc.ProductId);
        }

//////////////////////////////CATEGORY METHODS\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        [HttpGet("categories")]
        public IActionResult Categories()
        {
            List<Category> EveryCategory = _context.Categories.ToList();
            ViewBag.AllCategories = EveryCategory;
            return View("Categories");
        }

        [HttpPost("InsertCategory")]
        public IActionResult InsertCategory(Category newCatgry)
        {
            _context.Add(newCatgry);
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [HttpGet("categories/{catId}")]
        public IActionResult SpecificCategory(int catId)
        {
            Category getCat = _context.Categories.FirstOrDefault(p => p.CategoryId == catId);
            ViewBag.ThisCategory = getCat;

            var catWithProds = _context.Categories
                .Include(p => p.AssocProds)
                .ThenInclude(c => c.Product)
                .FirstOrDefault(p => p.CategoryId == catId);
            
            ViewBag.CategoryWithProducts = catWithProds;

            List<Product> EveryProduct = _context.Products.ToList();
            List<Product> SomeProducts = new List<Product>();

            foreach (var p in catWithProds.AssocProds)
            {
                SomeProducts.Add(p.Product);
            }
            List<Product> NotYetAssoc = EveryProduct.Except(SomeProducts).ToList();
            ViewBag.NotYetAssoc = NotYetAssoc;
            return View("SpecificCategory");
        }

        [HttpPost("AddProdToCat")]
        public IActionResult AddProdToCat(Association newAssoc)
        {
            _context.Add(newAssoc);
            _context.SaveChanges();
            return Redirect("/categories/"+newAssoc.CategoryId);
        }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
