using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Data;
using TestTask.Models;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;

namespace TestTask.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDescendantTask(int id, Models.Task task)
        {
            task.ParentTaskId = id;

            _context.Add(task);

            await _context.SaveChangesAsync();
        }

        public async Task AddDescendantTask(Models.Task parentTask, Models.Task descTask)
        {
            await AddDescendantTask(parentTask.Id, descTask);
        }

        public async Task<List<Models.Task>> GetClosestDescendantTasks(int id)
        {
            var tasks = await (from task in _context.Tasks
                               where task.ParentTaskId == id
                               select task).ToListAsync();

            return tasks;

        }

        public async Task<List<Models.Task>> GetClosestDescendantTasks(Models.Task task)
        {
            return await GetClosestDescendantTasks(task.Id);
        }

        public async Task<Models.Task> GetTaskById(int id)
        {
            return await _context.Tasks.FindAsync(id);  
        }

        // TODO: Add buisness logic 
        public async Task Edit(Models.Task task)
        {
            _context.Tasks.Update(task);

            await _context.SaveChangesAsync();
        }


    }
}
