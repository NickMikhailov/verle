using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Verle.ViewModels
{
    public class NomenclaturaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [StringLength(
            200,
            ErrorMessage = "Название не должно превышать 200 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите ID в 1С")]
        [StringLength(
            100,
            ErrorMessage = "ID в 1С не должен превышать 100 символов")]
        [Display(Name = "ID в 1С")]
        public string Id1C { get; set; } = string.Empty;

        [Display(Name = "Класс номенклатуры")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Выберите корректный класс")]
        public int? SelectedClassId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ClassOptions { get; set; }
            = new List<SelectListItem>();
        [Display(Name = "Класс SKU")]
        public int? SelectedSkuClassId { get; set; }

        [Display(Name = "Тип продукта")]
        public int? SelectedProductTypeId { get; set; }

        [Display(Name = "Бренд")]
        public int? SelectedBrandId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> SkuClassOptions { get; set; }
            = Array.Empty<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> ProductTypeOptions { get; set; }
            = Array.Empty<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> BrandOptions { get; set; }
            = Array.Empty<SelectListItem>();
    }
}