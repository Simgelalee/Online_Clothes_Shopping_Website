using System.ComponentModel.DataAnnotations;

namespace ClothesApp.Models
{
    public class Category

    {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public int ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set;}

        public string ParentCategoryDescription { get; set;}

        public string ImageUrl { get; set; }
    }
}
