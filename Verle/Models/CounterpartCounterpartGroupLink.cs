using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("counterpartCounterpartGroup")]
public class CounterpartCounterpartGroupLink
{
    [Key]
    [Column("counterpartId")]
    public int CounterpartId { get; set; }

    [Column("counterpartGroupId")]
    public int CounterpartGroupId { get; set; }

    public Counterpart Counterpart { get; set; } = null!;

    public CounterpartGroup CounterpartGroup { get; set; } = null!;
}