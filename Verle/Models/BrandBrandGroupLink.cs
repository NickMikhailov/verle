using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("brandBrandGroup")]
public class BrandBrandGroupLink
{
    [Key]
    [Column("brandId")]
    public int BrandId { get; set; }

    [Column("brandGroupId")]
    public int BrandGroupId { get; set; }

    public Brand Brand { get; set; } = null!;

    public BrandGroup BrandGroup { get; set; } = null!;
}