namespace Verle.Models;

public class NomenclaturaBrandLink
{
    public int NomenclaturaId { get; set; }

    public int BrandId { get; set; }

    public Nomenclatura Nomenclatura { get; set; } = null!;

    public Brand Brand { get; set; } = null!;
}