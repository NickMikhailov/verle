using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.ViewModels;

public class CounterpartFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ID контрагента из 1С.")]
    [StringLength(100)]
    [Display(Name = "ID в 1С")]
    public string Id1C { get; set; } = string.Empty;

    [StringLength(12)]
    [Display(Name = "ИНН")]
    public string? INN { get; set; }

    [Required(ErrorMessage = "Введите наименование контрагента.")]
    [StringLength(255)]
    [Display(Name = "Наименование")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Клиент")]
    public int? SelectedCounterpartGroupId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> CounterpartGroupOptions { get; set; }
        = Array.Empty<SelectListItem>();
}