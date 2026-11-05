namespace Verle.Models
{
    public class ProductionOrderExecution
    {
        // Одновременно первичный ключ и ссылка на заказ.
        public int OrderId { get; set; }

        public decimal Actual { get; set; }

        public string? Comment { get; set; }

        public ProductionOrder Order { get; set; } = null!;
    }
}