using System.ComponentModel.DataAnnotations;

namespace Verle.Models;

public class SkuClass
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите наименование класса SKU.")]
    [StringLength(255)]
    [Display(Name = "Класс SKU")]
    public string Name { get; set; } = string.Empty;

    public ICollection<NomenclaturaSkuClassLink> NomenclaturaLinks
    { get; set; } = new List<NomenclaturaSkuClassLink>();
}