namespace Verle.Models
{
    public class DripSemiproductOrderExecution
    {
        // Одновременно первичный ключ и ссылка на заказ.
        public int OrderId { get; set; }

        public decimal Actual { get; set; }

        public string? Comment { get; set; }

        public DripSemiproductOrder Order { get; set; } = null!;
    }
}