using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Must be a positive integer.")]
        public int Price { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Category can not be longer than 20 characters.")]
        public string Category { get; set; } = null!;
        public string Shelf { get; set; } = null!;
        public int Count { get; set; }
        [StringLength(500, ErrorMessage = "Description can not be longer than 500 characters.")]
        public string Description { get; set; } = null!;
    }
}
