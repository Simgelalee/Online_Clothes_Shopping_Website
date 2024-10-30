using Microsoft.EntityFrameworkCore.Query;

namespace ClothesApp.Models
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCars { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public IIncludableQueryable<ShoppingCart, Product> ListCart { get; internal set; }
    }
}
