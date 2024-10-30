using Clothes.Data;
using Microsoft.AspNetCore.Mvc;

namespace ClothesApp.Controllers
{
    public class ToolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToolsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Bag()
        {
            return View();
        }

        public IActionResult XBag()
        {

            var XBagProducts = _context.Products.Where(p => p.CategoryName == "Çapraz Çanta").ToList();
            return View(XBagProducts);
        }
        public IActionResult ShoulderBag()
        {

            var ShoulderBagProducts = _context.Products.Where(p => p.CategoryName == "Omuz Çantası").ToList();
            return View(ShoulderBagProducts);
        }
        public IActionResult Shirt()
        {
            return View();
        }
        public IActionResult ShortShirt()
        {

            var ShortShirtProducts = _context.Products.Where(p => p.CategoryName == "Kısa Etek").ToList();
            return View(ShortShirtProducts);
        }
        public IActionResult LongShirt()
        {

            var LongShirtProducts = _context.Products.Where(p => p.CategoryName == "Uzun Etek").ToList();
            return View(LongShirtProducts);
        }
        public IActionResult Shoes()
        {
            return View();
        }
        public IActionResult Sneakers()
        {
            var SneakersProducts = _context.Products.Where(p => p.CategoryName == "Sneakers").ToList();
            return View(SneakersProducts);
        }
        public IActionResult Sandals()
        {
            var SandalsProducts = _context.Products.Where(p => p.CategoryName == "Sandalet").ToList();
            return View(SandalsProducts);
        }

        public IActionResult Dress()
        {
            return View();
        }

        public IActionResult CasualDress()
        {

            var CasualDressProducts = _context.Products.Where(p => p.CategoryName == "Günlük Elbise").ToList();
            return View(CasualDressProducts);
        }

        public IActionResult EveningDress()
        {

            var EveningDressProducts = _context.Products.Where(p => p.CategoryName == "Gece Elbisesi").ToList();
            return View(EveningDressProducts);
        }

        public IActionResult Tshirt()
        {
            return View();
        }
        public IActionResult _Tshirt()
        {

            var TshirtProducts = _context.Products.Where(p => p.CategoryName == "Tişört").ToList();
            return View(TshirtProducts);


        }
    }
}
