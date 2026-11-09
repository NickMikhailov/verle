using System.ComponentModel.DataAnnotations;

namespace Verle.Models
{
    public static class ExecutionStatusIds
    {
        public const int InProgress = 1;
        public const int Downtime = 2;
        public const int Finished = 3;
    }

    public class ExecutionStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}