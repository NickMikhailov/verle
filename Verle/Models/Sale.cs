using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models;

[Table("sales")]
public class Sale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    // Наш внутренний Id контрагента.
    [Column("counterpartId")]
    public int CounterpartId { get; set; }

    // Код контрагента в 1С, сохранённый как источник.
    [Required]
    [StringLength(100)]
    [Column("counterpartId1C")]
    public string CounterpartId1C { get; set; } = string.Empty;

    // Наименование контрагента именно из файла 1С.
    [Required]
    [StringLength(255)]
    [Column("counterpartName1C")]
    public string CounterpartName1C { get; set; } = string.Empty;

    [Required]
    [Column("date", TypeName = "date")]
    [Display(Name = "Дата продажи")]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(500)]
    [Column("document")]
    [Display(Name = "Документ")]
    public string Document { get; set; } = string.Empty;

    // Наш внутренний Id номенклатуры.
    [Column("nomenclaturaId")]
    public int NomenclaturaId { get; set; }

    // Код номенклатуры в 1С.
    [Required]
    [StringLength(100)]
    [Column("nomenclaturaId1C")]
    public string NomenclaturaId1C { get; set; } = string.Empty;

    // Наименование номенклатуры из файла 1С.
    [Required]
    [StringLength(255)]
    [Column("nomenclaturaName1C")]
    public string NomenclaturaName1C { get; set; } = string.Empty;

    [Column("quantity", TypeName = "double precision")]
    public double Quantity { get; set; }

    [Column("revenue", TypeName = "double precision")]
    public double Revenue { get; set; }

    [Column("cost", TypeName = "double precision")]
    public double Cost { get; set; }

    [Column("grossProfit", TypeName = "double precision")]
    public double GrossProfit { get; set; }

    [Column("margin", TypeName = "double precision")]
    public double Margin { get; set; }

    [ForeignKey(nameof(CounterpartId))]
    public Counterpart Counterpart { get; set; } = null!;

    [ForeignKey(nameof(NomenclaturaId))]
    public Nomenclatura Nomenclatura { get; set; } = null!;
}