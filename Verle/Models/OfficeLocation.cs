using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class OfficeLocation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Локация офиса")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Локация")]
        public string ShortName { get; set; } = string.Empty;
    }
}