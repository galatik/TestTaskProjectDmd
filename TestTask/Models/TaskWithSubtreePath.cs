using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask.Models
{
    public class TaskWithSubtreePath
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Performers { get; set; }

        public DateTime CreationDate { get; set; }

        public TaskStatus Status { get; set; }

        public int PlannedLaboriousness { get; set; }

        public int PlannedLaboriousnessWithDescendant { get; set; }

        public TimeSpan CompletionTime { get; set; }

        public TimeSpan CompletionTimeWithDescendant { get; set; }

        public DateTime ActualCompletionDate { get; set; }

        public string Path { get; set; }
    }
}
