using System.ComponentModel.DataAnnotations;

namespace Verle.Models;

public class ProductType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите наименование типа продукта.")]
    [StringLength(255)]
    [Display(Name = "Тип продукта")]
    public string Name { get; set; } = string.Empty;

    public ICollection<NomenclaturaProductTypeLink> NomenclaturaLinks
    { get; set; } = new List<NomenclaturaProductTypeLink>();
}