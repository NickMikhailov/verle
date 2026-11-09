using System;

namespace Verle.ViewModels
{
    public class ProductionOrderExecutionEventViewModel
    {
        public long Id { get; set; }

        public DateTime OccurredAt { get; set; }

        public int ExecutionStatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;
    }
}