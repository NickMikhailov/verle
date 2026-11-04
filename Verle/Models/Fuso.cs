using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Fuso
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Номер машины")]
        public string FusoNumber { get; set; } = string.Empty;

        [Display(Name = "Дата ввода в эксплуатацию")]
        public DateOnly CommissioningDate { get; set; }

        [Display(Name = "Максимальная производительность")]
        public double Capacity { get; set; }

        [Display(Name = "Единица измерения")]
        public int UnitId { get; set; }

        public Unit Unit { get; set; } = null!;
    }
}