using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Division
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название отдела.")]
        [StringLength(
            100,
            ErrorMessage = "Название отдела не должно превышать 100 символов.")]
        [Display(Name = "Отдел")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите блок.")]
        [Display(Name = "Блок")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}