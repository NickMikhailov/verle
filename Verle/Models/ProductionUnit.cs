using System.ComponentModel.DataAnnotations;

namespace Verle.Models;

public class ProductionUnit
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Производственный участок")]
    public string Name { get; set; } = string.Empty;
}