using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
    public class Task
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        
        [MaxLength(300)]
        public string Performers { get; set; }

        public DateTime CreationDate { get; set; }

        public TaskStatus Status { get; set; }

        public int PlannedLaboriousness { get; set; }

        [NotMapped]
        public int PlannedLaboriousnessWithDescendant { get; set; }

        public TimeSpan CompletionTime { get; set; }

        [NotMapped]
        public TimeSpan CompletionTimeWithDescendant { get; set; }

        public DateTime? ActualCompletionDate { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

        public int? ParentTaskId { get; set; }

        public Task ParentTask { get; set; }
    }
}
