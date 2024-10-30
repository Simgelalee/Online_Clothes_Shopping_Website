using Clothes.Data;
using Clothes.Models;
using ClothesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ClothesApp.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private string? cart;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Where(i => i.IsHome).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _context.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count);
            }
            return View(products);
        }
        public IActionResult Details(int id)
        {
          
            var product = _context.Products.FirstOrDefault(i => i.Id == id);

            if (product == null)
                return NotFound();

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };

            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart Scart)
        {    var errors = ModelState.Values.SelectMany(x=> x.Errors);
            
            if (ModelState.IsValid)
            
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                Scart.ApplicationUserId = claim.Value;
                ShoppingCart cart = _context.ShoppingCarts.Where(
                    u => u.ApplicationUserId == Scart.ApplicationUserId && u.ProductId == Scart.ProductId).FirstOrDefault();
                if (cart == null)
                {
                    _context.ShoppingCarts.Add(Scart);
                }
                else
                {
                    cart.Count += Scart.Count;
                }
                Debug.WriteLine(Scart);
                Debug.WriteLine(cart);
                //Debug.WriteLine(cart);
                _context.SaveChanges();
                var count = _context.ShoppingCarts.Where(i => i.ApplicationUserId == Scart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _context.Products.FirstOrDefault(i => i.Id == Scart.Id);
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
            }

            return View(Scart);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
