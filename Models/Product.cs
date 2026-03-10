using System.ComponentModel.DataAnnotations;

namespace WpfApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
