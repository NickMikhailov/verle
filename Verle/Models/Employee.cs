using System;
using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Имя")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }

        [Display(Name = "Дата приёма на работу")]
        public DateOnly HireDate { get; set; }

        [Display(Name = "Должность")]
        public int PositionId { get; set; }

        public Position Position { get; set; } = null!;
    }
}