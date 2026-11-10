namespace Verle.ViewModels
{
    public class UpdateProductionOrderPlanRequest
    {
        public int Id { get; set; }

        // Передаём строкой, чтобы при сохранении decimal
        // не терять точность через JavaScript Number.
        public string? Plan { get; set; }
    }
}