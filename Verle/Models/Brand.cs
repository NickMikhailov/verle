using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("brand")]
public class Brand
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [Display(Name = "Бренд")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<NomenclaturaBrandLink> NomenclaturaLinks { get; set; }
        = new List<NomenclaturaBrandLink>();

    public BrandBrandGroupLink? BrandGroupLink { get; set; }
}