using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class ProductionOrder
    {
        public int Id { get; set; }

        [Display(Name = "ID заказа в 1С")]
        public string? Id1C { get; set; }

        [Display(Name = "Дата заказа")]
        public DateOnly? OrderDate { get; set; }

        [Display(Name = "Номенклатура")]
        public int? NomenclaturaId { get; set; }

        public Nomenclatura? Nomenclatura { get; set; }

        [Display(Name = "Смена")]
        public int? ShiftId { get; set; }

        public Shift? Shift { get; set; }

        [Display(Name = "Оборудование")]
        public int? EquipmentId { get; set; }

        public Equipment? Equipment { get; set; }

        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Display(Name = "План")]
        public decimal? Plan { get; set; }

        public ProductionOrderExecution? Execution { get; set; }

        // 1 — Редактируется, 2 — Создан.
        public int StatusId { get; set; } = 1;

        public OrderStatus Status { get; set; } = null!;
    }
}