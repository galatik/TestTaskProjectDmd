using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Data;
using TestTask.Models;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Data;

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

        public async Task<int> GetPlannedLaboriousnessWithDescendant(int taskid)
        {
            var plannedLabparam = new SqlParameter()
            {
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output,
                ParameterName = "@result",
                Size = 11
            };

            await _context.Database.ExecuteSqlCommandAsync($"call getPlannedLabOfDesc({taskid},@result)", plannedLabparam);

            return (int) plannedLabparam.Value;

        }

        public async Task<int> GetPlannedLaboriousnessWithDescendant(Models.Task task)
        {
            return await GetPlannedLaboriousnessWithDescendant(task.Id);
        }

        public async Task<TimeSpan> GetCompletionTimeWithDescendant(int taskid)
        {
            var compTimeparam = new SqlParameter()
            {
                SqlDbType = SqlDbType.Time,
                Direction = ParameterDirection.Output,
                ParameterName = "@result",
                Size = 6
            };

            await _context.Database.ExecuteSqlCommandAsync($"call getCompletionTimeOfDesc({taskid}, @result)", compTimeparam);

            return (TimeSpan)compTimeparam.Value;
        }

        public async Task<Tree<Models.TaskWithSubtreePath>> GetFullTaskTree(int taskid)
        {
            var tasksWithPath = await _context.taskWithSubtreePaths.FromSql($"call getAllTaskDescendants({taskid})").ToListAsync();

            var headTask = tasksWithPath.FirstOrDefault(t => t.Id == taskid);

            var headNode = new Node<Models.TaskWithSubtreePath>();

            headNode.Value = headTask;

            headNode.SubNodes = getAllDescendants(headNode, tasksWithPath);

            return new Tree<Models.TaskWithSubtreePath>()
            {
                HeadNode = headNode
            };

            
        }

        private List<Node<Models.TaskWithSubtreePath>> getAllDescendants(Node<Models.TaskWithSubtreePath> headNode, List<TaskWithSubtreePath> descList)
        {
            var subNodesList = new List<Node<Models.TaskWithSubtreePath >>();

            var closestDesc = descList.Where(t => t.ParentTaskId == headNode.Value.Id).ToList();

            descList.RemoveAll(t => closestDesc.Contains(t));

            foreach (var task in closestDesc)
            {
                var taskNode = new Node<Models.TaskWithSubtreePath>()
                {
                    Value = task,
                    ParentNode = headNode
                };

                taskNode.SubNodes = getAllDescendants(taskNode, descList);
                subNodesList.Add(taskNode);
            }

            return subNodesList;
        }

    }
}
