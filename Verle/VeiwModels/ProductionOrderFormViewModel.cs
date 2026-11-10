using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.ViewModels
{
    public class ProductionOrderFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "ID заказа в 1С")]
        public string? Id1C { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата заказа")]
        public DateOnly? OrderDate { get; set; }

        [Display(Name = "Номенклатура")]
        public int? NomenclaturaId { get; set; }

        [Display(Name = "Смена")]
        public int? ShiftId { get; set; }

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "999999999999999.999",
            ErrorMessage =
                "План должен быть неотрицательным и помещаться в numeric(18,3).",
            ParseLimitsInInvariantCulture = true)]
        [Display(Name = "План")]
        public decimal? Plan { get; set; }
    }
}