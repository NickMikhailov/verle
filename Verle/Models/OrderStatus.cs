using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}