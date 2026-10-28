using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Verle.Models
{
    [Table("nomenclatura")]
    public class Nomenclatura
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "ID в 1С")]
        [StringLength(100)]
        [Column("id1C")]
        public string Id1C { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Введите название")]
        [StringLength(
            200,
            ErrorMessage = "Название не должно превышать 200 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        public NomenclaturaClassLink? ClassLink { get; set; }
        
        public NomenclaturaSkuClassLink? SkuClassLink { get; set; }

        public NomenclaturaProductTypeLink? ProductTypeLink { get; set; }

        public NomenclaturaBrandLink? BrandLink { get; set; }
    }
}