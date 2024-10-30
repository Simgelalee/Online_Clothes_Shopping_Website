using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesApp.Models
{
    public class Product
    {
       
            [Key]
            public int Id { get; set; }
            [ValidateNever]
            [Required(ErrorMessage = "Ürün adı gereklidir.")]
            [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olmalıdır.")]

            public string Name { get; set; }
            [ValidateNever]
            [Required(ErrorMessage = "Ürün açıklaması gereklidir.")]
            public string Description { get; set; }
            [ValidateNever]
            [Required(ErrorMessage = "Ürün fiyatı gereklidir.")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Ürün fiyatı 0.01'den büyük olmalıdır.")]
            
            public int Price { get; set; }
            [ValidateNever]
            public bool IsHome { get; set; }
            [ValidateNever]
            public string ImageUrl { get; set; }
            [ValidateNever]
            public string CategoryName { get; set; }

            [Required(ErrorMessage = "Kategori ID gereklidir.")]
            [ValidateNever]
            public int CategoryId { get; set; }

           
            [ValidateNever]
            [ForeignKey("CategoryId")]
            public Category Category { get; set; }
            [ValidateNever]

            [Required(ErrorMessage = "Marka adı gereklidir.")]
            [StringLength(50, ErrorMessage = "Marka adı en fazla 50 karakter olmalıdır.")]
            public string Brand { get; set; }
            [ValidateNever]

            [StringLength(100, ErrorMessage = "Model adı en fazla 100 karakter olmalıdır.")]
            public string Model { get; set; }

            [StringLength(50, ErrorMessage = "Renk adı en fazla 50 karakter olmalıdır.")]
            [ValidateNever]
            public string Color { get; set; }
            [ValidateNever]

            [StringLength(50, ErrorMessage = "Beden adı en fazla 50 karakter olmalıdır.")]
            public string Size { get; set; }
            [ValidateNever]

            public int StockQuantity { get; set; } = 0; // Stok miktarı, varsayılan olarak 0
            [ValidateNever]

            public bool IsActive { get; set; } = true; // Ürün aktif mi?
            [ValidateNever]

            public DateTime CreatedAt { get; set; } = DateTime.Now; // Oluşturma tarihi
        }
    }



