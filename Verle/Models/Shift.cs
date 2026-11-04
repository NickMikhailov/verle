using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Смена")]
        public string Name { get; set; } = string.Empty;
    }
}