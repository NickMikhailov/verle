using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.ViewModels
{
    public class DripSemiproductOrderFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите ID заказа в 1С.")]
        [Display(Name = "ID заказа в 1С")]
        public string Id1C { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите дату заказа.")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата заказа")]
        public DateOnly? OrderDate { get; set; } =
            DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "Выберите номенклатуру.")]
        [Display(Name = "Номенклатура")]
        public int? NomenclaturaId { get; set; }

        [Required(ErrorMessage = "Выберите смену.")]
        [Display(Name = "Смена")]
        public int? ShiftId { get; set; } = 1;

        [Required(ErrorMessage = "Выберите машину FUSO.")]
        [Display(Name = "Машина FUSO")]
        public int? FusoId { get; set; }

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Укажите плановое количество.")]
        [Range(
            typeof(decimal),
            "0",
            "999999999999999.999",
            ErrorMessage = "План должен быть неотрицательным и помещаться в numeric(18,3).")]
        [Display(Name = "План")]
        public decimal? Plan { get; set; }
    }
}