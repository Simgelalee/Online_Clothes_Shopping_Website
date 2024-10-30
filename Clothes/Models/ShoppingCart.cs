using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesApp.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }
        [ValidateNever]
        public string ApplicationUserId{ get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public  ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [ValidateNever]
        public int Count { get; set; }
        [ValidateNever]
        [NotMapped]
        public double Price { get; set; }


    }
}
