using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class ProductionOrder
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

        [Display(Name = "Оборудование")]
        public int EquipmentId { get; set; }

        public Equipment Equipment { get; set; } = null!;

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Display(Name = "План")]
        public decimal Plan { get; set; }
        public ProductionOrderExecution? Execution { get; set; }
        public int StatusId { get; set; } = 2;
        public OrderStatus Status { get; set; } = null!;
    }
}