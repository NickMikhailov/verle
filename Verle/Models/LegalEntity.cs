using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class LegalEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Полное наименование")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Краткое наименование")]
        public string ShortName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Display(Name = "ИНН")]
        public string INN { get; set; } = string.Empty;
    }
}