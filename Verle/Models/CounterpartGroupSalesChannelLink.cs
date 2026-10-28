using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("counterpartGroupSalesChannel")]
public class CounterpartGroupSalesChannelLink
{
    [Key]
    [Column("counterpartGroupId")]
    public int CounterpartGroupId { get; set; }

    [Column("salesChannelId")]
    public int SalesChannelId { get; set; }

    public CounterpartGroup CounterpartGroup { get; set; } = null!;

    public SalesChannel SalesChannel { get; set; } = null!;
}