using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование оборудования")]
        public string EquipmentName { get; set; } = string.Empty;

        [Display(Name = "Дата ввода в эксплуатацию")]
        public DateOnly CommissioningDate { get; set; }

        [Display(Name = "Максимальная производительность")]
        public double Capacity { get; set; }

        [Display(Name = "Единица измерения")]
        public int UnitId { get; set; }

        public Unit Unit { get; set; } = null!;
    }
}