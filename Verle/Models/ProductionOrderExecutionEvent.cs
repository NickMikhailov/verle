using System;

namespace Verle.Models
{
    public class ProductionOrderExecutionEvent
    {
        public long Id { get; set; }

        public int OrderId { get; set; }

        // Всегда UTC.
        public DateTime OccurredAt { get; set; }

        public int ExecutionStatusId { get; set; }

        public ProductionOrder Order { get; set; } = null!;

        public ExecutionStatus ExecutionStatus { get; set; } = null!;
    }
}