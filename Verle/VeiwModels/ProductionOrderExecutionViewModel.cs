using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace Verle.ViewModels
{
    public class ProductionOrderExecutionViewModel
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

        public DateTime? StartedAt { get; set; }
        public string? OrderComment { get; set; }
        // Данные журнала только для отображения.
        // Не принимаем их как источник состояния при сохранении факта.
        [ValidateNever]
        public int? CurrentExecutionStatusId { get; set; }

        [ValidateNever]
        public long? LastExecutionEventId { get; set; }

        [ValidateNever]
        public List<ProductionOrderExecutionEventViewModel> Events { get; set; }
            = new();
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