using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.Models
{
    public class PositionFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название должности.")]
        [StringLength(
            100,
            ErrorMessage = "Название не должно превышать 100 символов.")]
        [Display(Name = "Должность")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите блок.")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите блок.")]
        [Display(Name = "Блок")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Выберите отдел.")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите отдел.")]
        [Display(Name = "Отдел")]
        public int? DivisionId { get; set; }

        [ValidateNever]
        public List<SelectListItem> DepartmentOptions { get; set; }
            = new();

        [ValidateNever]
        public List<SelectListItem> DivisionOptions { get; set; }
            = new();
    }
}