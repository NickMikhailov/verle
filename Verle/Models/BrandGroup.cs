using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("brandGroup")]
public class BrandGroup
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название группы брендов.")]
    [StringLength(
        255,
        ErrorMessage = "Название группы не должно превышать 255 символов.")]
    [Display(Name = "Название группы")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<BrandBrandGroupLink> BrandLinks { get; set; }
        = new List<BrandBrandGroupLink>();
}