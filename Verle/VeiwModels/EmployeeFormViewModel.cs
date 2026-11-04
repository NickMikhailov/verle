using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Выберите блок.")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите блок.")]
        [Display(Name = "Блок")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Выберите отдел.")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите отдел.")]
        [Display(Name = "Отдел")]
        public int? DivisionId { get; set; }

        [Required(ErrorMessage = "Выберите должность.")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите должность.")]
        [Display(Name = "Должность")]
        public int? PositionId { get; set; }

        [Required(ErrorMessage = "Введите фамилию.")]
        [StringLength(100)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите имя.")]
        [StringLength(100)]
        [Display(Name = "Имя")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Отчество")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Укажите дату приёма на работу.")]
        [DataType(DataType.Date)]
        [DisplayFormat(
            DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        [Display(Name = "Дата приёма на работу")]
        public DateOnly? HireDate { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();

        public List<SelectListItem> Divisions { get; set; } = new();

        public List<SelectListItem> Positions { get; set; } = new();
    }
}