using System.ComponentModel.DataAnnotations;

namespace Verle.Models;

public class ProductionSpecification
{
    public int Id { get; set; }

    [Display(Name = "Производимая номенклатура")]
    public int NomenclaturaIdOutput { get; set; }

    [Display(Name = "Входной компонент")]
    public int NomenclaturaIdInput { get; set; }

    [Display(Name = "Количество компонента")]
    public decimal QuantityInput { get; set; }

    [Display(Name = "Количество результата")]
    public decimal QuantityOutput { get; set; }

    public Nomenclatura NomenclaturaOutput { get; set; } = null!;

    public Nomenclatura NomenclaturaInput { get; set; } = null!;
}