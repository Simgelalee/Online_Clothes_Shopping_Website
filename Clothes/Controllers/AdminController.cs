using Microsoft.AspNetCore.Mvc;

namespace ClothesApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
