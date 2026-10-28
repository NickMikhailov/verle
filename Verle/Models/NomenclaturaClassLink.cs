using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models
{
    [Table("nomenclaturaClassLink")]
    public class NomenclaturaClassLink
    {
        [Column("nomenclaturaId")]
        public int NomenclaturaId { get; set; }

        [Column("nomenclaturaClassId")]
        public int NomenclaturaClassId { get; set; }

        public Nomenclatura Nomenclatura { get; set; } = null!;

        public NomenclaturaClass NomenclaturaClass { get; set; } = null!;
    }
}