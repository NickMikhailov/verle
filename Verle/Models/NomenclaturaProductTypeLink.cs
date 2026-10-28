namespace Verle.Models;

public class NomenclaturaProductTypeLink
{
    public int NomenclaturaId { get; set; }

    public int ProductTypeId { get; set; }

    public Nomenclatura Nomenclatura { get; set; } = null!;

    public ProductType ProductType { get; set; } = null!;
}