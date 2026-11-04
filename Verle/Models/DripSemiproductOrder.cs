using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class DripSemiproductOrder
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор заказа в 1С.")]
        [Display(Name = "ID заказа в 1С")]
        public string Id1C { get; set; } = string.Empty;

        [Display(Name = "Дата заказа")]
        public DateOnly OrderDate { get; set; }

        [Display(Name = "Номенклатура")]
        public int NomenclaturaId { get; set; }

        public Nomenclatura Nomenclatura { get; set; } = null!;

        [Display(Name = "Смена")]
        public int ShiftId { get; set; } = 1;

        public Shift Shift { get; set; } = null!;

        [Display(Name = "Машина FUSO")]
        public int FusoId { get; set; }

        public Fuso Fuso { get; set; } = null!;

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Display(Name = "План")]
        public decimal Plan { get; set; }
        public DripSemiproductOrderExecution? Execution { get; set; }
    }
}