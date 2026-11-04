using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.ViewModels
{
    public class DripSemiproductOrderExecutionViewModel
    {
        public int Id { get; set; }

        // Данные заказа для отображения.
        public string Id1C { get; set; } = string.Empty;

        public DateOnly OrderDate { get; set; }

        public string NomenclaturaId1C { get; set; } = string.Empty;

        public string NomenclaturaName { get; set; } = string.Empty;

        public string ShiftName { get; set; } = string.Empty;

        public string FusoNumber { get; set; } = string.Empty;

        public decimal Plan { get; set; }

        public string? OrderComment { get; set; }

        // Редактируемые данные выполнения.
        [Display(Name = "Факт")]
        [Required(ErrorMessage = "Укажите фактическое количество.")]
        [Range(
            typeof(decimal),
            "0",
            "999999999999999.999",
            ErrorMessage =
                "Факт должен быть от 0 до 999999999999999,999.",
            ParseLimitsInInvariantCulture = true)]
        public decimal? Actual { get; set; }

        [Display(Name = "Комментарий к выполнению")]
        public string? Comment { get; set; }
    }
}