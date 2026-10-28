using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.ViewModels;

public class BrandFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название бренда.")]
    [StringLength(
        255,
        ErrorMessage = "Название бренда не должно превышать 255 символов.")]
    [Display(Name = "Бренд")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Группа брендов")]
    public int? SelectedBrandGroupId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> BrandGroupOptions { get; set; }
        = Array.Empty<SelectListItem>();
}