using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Verle.Models
{
    public class Position
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название должности.")]
        [StringLength(
            100,
            ErrorMessage = "Название не должно превышать 100 символов.")]
        [Display(Name = "Должность")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите отдел.")]
        [Display(Name = "Отдел")]
        public int DivisionId { get; set; }

        [ValidateNever]
        public Division Division { get; set; } = null!;
    }
}