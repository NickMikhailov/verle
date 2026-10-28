using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models
{
    [Table("nomenclaturaClass")]
    public class NomenclaturaClass
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Введите название класса")]
        [StringLength(
            100,
            ErrorMessage = "Название не должно превышать 100 символов")]
        [Display(Name = "Название класса")]
        public string Name { get; set; } = string.Empty;

        public ICollection<NomenclaturaClassLink> NomenclaturaLinks
        { get; set; } = new List<NomenclaturaClassLink>();
    }
}