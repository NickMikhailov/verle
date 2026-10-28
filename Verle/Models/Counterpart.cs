using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("counterpart")]
public class Counterpart
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ID контрагента из 1С.")]
    [StringLength(
        100,
        ErrorMessage = "ID из 1С не должен превышать 100 символов.")]
    [Display(Name = "ID в 1С")]
    [Column("id1C")]
    public string Id1C { get; set; } = string.Empty;

    [StringLength(
        12,
        ErrorMessage = "ИНН не должен превышать 12 символов.")]
    [Display(Name = "ИНН")]
    [Column("INN")]
    public string? INN { get; set; }

    [Required(ErrorMessage = "Введите наименование контрагента.")]
    [StringLength(
        255,
        ErrorMessage = "Наименование не должно превышать 255 символов.")]
    [Display(Name = "Наименование")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public CounterpartCounterpartGroupLink?
    CounterpartGroupLink
    { get; set; }
}