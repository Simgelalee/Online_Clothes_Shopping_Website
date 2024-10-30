using Clothes.Data;
using ClothesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace ClothesApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles = Diger.Role_Admin)]
        public IActionResult Onaylandi()
        {    
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(i => i.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = Diger.Durum_Onaylandi;
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = Diger.Role_Admin)]
        public IActionResult KargoyaVer()
        {
            OrderHeader orderHeader = _context.OrderHeaders.FirstOrDefault(i => i.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = Diger.Durum_Kargoda;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM
            {
                OrderHeader = _context.OrderHeaders.FirstOrDefault(i => i.Id == id),
                OrderDetails = _context.OrderDetails.Where(x => x.OrderId == id).Include(x => x.Product)

            };
            if (OrderVM.OrderHeader.OrderStatus == null)
            {
                OrderVM.OrderHeader.OrderStatus = "Pending"; 
            }

            return View(OrderVM);
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.ToList();
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.ToList().Where(i => i.ApplicationUserId == claim.Value.ToString());
            }
            return View(orderHeadersList);
        }
        public IActionResult Beklenen()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Beklemede);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.ToList().Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Diger.Durum_Beklemede);
                   
            }
            return View(orderHeadersList);
        }
        public IActionResult Onaylanan()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Onaylandi);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.ToList().Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Diger.Durum_Onaylandi);
                    
            }
            return View(orderHeadersList);
        }
       

        public IActionResult Kargolanan()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Diger.Role_Admin))
            {
                orderHeadersList = _context.OrderHeaders.Where(i => i.OrderStatus == Diger.Durum_Kargoda);
            }
            else
            {
                orderHeadersList = _context.OrderHeaders.ToList().Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Diger.Durum_Kargoda);
                   
            }
            return View(orderHeadersList);
        }
    }

   
}
