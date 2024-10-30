using Clothes.Data;
using ClothesApp.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClothesApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager <IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _context.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.ListCart = _context.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product);
            ShoppingCartVM.OrderHeader.OrderStatus = Diger.Durum_Beklemede;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            _context.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
            _context.SaveChanges();
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _context.OrderDetails.Add(orderDetails);
            }
            var payment = PaymentProcess(model);
            _context.ShoppingCarts.RemoveRange(ShoppingCartVM.ListCart);
            _context.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, 0);
            return RedirectToAction("SiparisTamam");
        }

        private Payment PaymentProcess(ShoppingCartVM model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-DoPyydI90nTM2ouY1GgCIb1A5ZjI8hTa";
            options.SecretKey = "sandbox-UDWh6V7OeeFZql6s4xQAheF7ziGvfqK9";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111, 9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.CartName;
            paymentCard.CardNumber = model.OrderHeader.CartNumber;
            paymentCard.ExpireMonth = model.OrderHeader.ExpirationMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpirationYear;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.Id.ToString();
            buyer.Name = model.OrderHeader.Name;
            buyer.Surname = model.OrderHeader.Surname;
            buyer.GsmNumber = model.OrderHeader.PhoneNumber;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Adres;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.Sehir;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.PostaKodu;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var item in _context.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product))
            {
                basketItems.Add(new BasketItem()
                {

                    Id = item.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = item.Product.CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Count).ToString()
                });
            }
            request.BasketItems = basketItems;

            return Payment.Create(request, options);
        }


        public IActionResult SiparisTamam()
        {
            // Sipariş tamamlandığında stok güncelleme işlemini burada gerçekleştirin

            // Kullanıcı kimliğini alın
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Kullanıcının alışveriş sepetindeki ürünleri alın
            var cartItems = _context.ShoppingCarts
                                    .Where(i => i.ApplicationUserId == claim.Value)
                                    .Include(i => i.Product)
                                    .ToList();

            // Stok güncellemesini gerçekleştirin
            foreach (var item in cartItems)
            {
                // Ürünün stok miktarını güncelleyin
                var product = item.Product;
                product.StockQuantity -= item.Count;

                // Stok miktarı sıfırın altına düşerse gerekli işlemi yapın
                if (product.StockQuantity < 0)
                {
                    // Hata durumu veya stok yetersizliği durumunda gerekli işlemler
                    return View("View"); // Örneğin stok yetersizliği sayfasına yönlendirin
                }

                _context.Products.Update(product);
            }

            // Değişiklikleri veritabanına kaydedin
            _context.SaveChanges();

            // Sipariş tamamlama işlemleri
            return View("SiparisTamam");
        }
        [HttpPost]
        public IActionResult SiparisTamam(ShoppingCartVM shoppingCartVM)
        {
            // Sipariş tamamlama işlemlerini burada gerçekleştirin
            // Ödeme işlemlerini gerçekleştirin

            // Stok güncelleme işlemleri
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cartItems = _context.ShoppingCarts
                                    .Where(i => i.ApplicationUserId == claim.Value)
                                    .Include(i => i.Product)
                                    .ToList();

            foreach (var item in cartItems)
            {
                var product = item.Product;
                product.StockQuantity -= item.Count;

                if (product.StockQuantity < 0)
                {
                    return View("View");
                }

                _context.Products.Update(product);
            }

            _context.SaveChanges();

            // Sipariş tamamlama işlemleri
            return View("SiparisTamam");
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _context.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)


            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach(var item in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Add(int cartId)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            cart.Count += 1;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cartId)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            if (cart.Count == 1)
            {
                var count = _context.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _context.ShoppingCarts.Remove(cart);
                _context.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);

            var count = _context.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);


            return RedirectToAction(nameof(Index));

        }

    }
}
