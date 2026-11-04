using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название блока.")]
        [StringLength(
            100,
            ErrorMessage = "Название блока не должно превышать 100 символов.")]
        [Display(Name = "Блок")]
        public string Name { get; set; } = string.Empty;
    }
}