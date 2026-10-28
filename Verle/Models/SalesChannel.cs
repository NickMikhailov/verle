using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("salesChannel")]
public class SalesChannel
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название канала продаж.")]
    [StringLength(
        255,
        ErrorMessage = "Название канала не должно превышать 255 символов.")]
    [Display(Name = "Канал продаж")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<CounterpartGroupSalesChannelLink>
        CounterpartGroupLinks
    { get; set; }
        = new List<CounterpartGroupSalesChannelLink>();
}