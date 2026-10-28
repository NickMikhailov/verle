namespace Verle.Models;

public class NomenclaturaSkuClassLink
{
    public int NomenclaturaId { get; set; }

    public int SkuClassId { get; set; }

    public Nomenclatura Nomenclatura { get; set; } = null!;

    public SkuClass SkuClass { get; set; } = null!;
}