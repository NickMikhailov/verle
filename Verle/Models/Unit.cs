using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название единицы измерения.")]
        [StringLength(
            100,
            ErrorMessage = "Название не должно превышать 100 символов.")]
        [Display(Name = "Единица измерения")]
        public string Name { get; set; } = string.Empty;
    }
}