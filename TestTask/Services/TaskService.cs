using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Data;
using TestTask.Models;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace TestTask.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;


        public TaskService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task CreateTask(Models.Task task, string userId)
        {
            task.UserId = userId;

            _context.Add(task);

            await _context.SaveChangesAsync();
        }

        public IQueryable<Models.Task> GetClosestDescendantTasks(int id)
        {
            var tasks =  from task in _context.Tasks
                         where task.ParentTaskId == id
                         select task;

            return tasks;

        }

        public IQueryable<Models.Task> GetClosestDescendantTasks(Models.Task task)
        {
            return GetClosestDescendantTasks(task.Id);
        }

        public async Task<Models.Task> GetTaskById(int id)
        {
            return await _context.Tasks.FindAsync(id);  
        }

        
        public async Task DeleteTask(int id)
        {
            var task = await GetTaskById(id);

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();
        }


        public async Task Edit(Models.Task task)
        {
            var oldTask = await GetTaskById(task.Id);

            var prevStatus = oldTask.Status;


            if(task.Status == Models.TaskStatus.Completed && prevStatus !=  Models.TaskStatus.Completed)
            {
                var tasksWithPath =  _context.taskWithSubtreePaths.FromSql($"call getAllTaskDescendants({task.Id})");

                bool canBeCompleted = await (from taskWithPath in tasksWithPath
                                              join t in _context.Tasks
                                              on taskWithPath.Id equals t.Id
                                              where taskWithPath.Id != task.Id
                                              select t.Status)
                                              .AllAsync(s => s == Models.TaskStatus.InPerform || s == Models.TaskStatus.Suspended || s == Models.TaskStatus.Completed);

                if (canBeCompleted)
                {
                    var tasks = await (from taskWP in tasksWithPath
                                join tk in _context.Tasks
                                on taskWP.Id equals tk.Id
                                select tk).ToListAsync();

                    foreach (var tsk in tasks)
                    {
                        tsk.Status = Models.TaskStatus.Completed;
                    }

                    _context.Tasks.UpdateRange(tasks);

                    task.ActualCompletionDate = DateTime.Now;

                } else
                {
                    task.Status = oldTask.Status;
                }
            }

            oldTask.Status = task.Status;

            oldTask.Performers = task.Performers;

            oldTask.Name = task.Name;

            oldTask.CompletionTime = task.CompletionTime;

            oldTask.ActualCompletionDate = task.ActualCompletionDate;

            oldTask.Description = task.Description;

            oldTask.PlannedLaboriousness = task.PlannedLaboriousness;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetPlannedLaboriousnessWithDescendant(int taskid)
        {

            var inParam = new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                Direction = ParameterDirection.Input,
                ParameterName = "@taskid",
                Size = 11,
                Value = taskid
            };

            var outParam = new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                Direction = ParameterDirection.Output,
                ParameterName = "@result",
                Size = 11
            };

            var parameters = new List<MySqlParameter>()
            {
                inParam,
                outParam
            };

            await _context.CallProcedure("getPlannedLabOfDesc", parameters);

            return (int) outParam.Value;
            
        }

    

        public async Task<int> GetPlannedLaboriousnessWithDescendant(Models.Task task)
        {
            return await GetPlannedLaboriousnessWithDescendant(task.Id);
        }

        public async Task<TimeSpan> GetCompletionTimeWithDescendant(int taskid)
        {
            var inParam = new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                Direction = ParameterDirection.Input,
                ParameterName = "@taskid",
                Size = 11,
                Value = taskid
            };

            var outParam = new MySqlParameter()
            {
                MySqlDbType = MySqlDbType.Int32,
                Direction = ParameterDirection.Output,
                ParameterName = "@result"
            };

            var parameters = new List<MySqlParameter>()
            {
                inParam,
                outParam
            };

            await _context.CallProcedure("getCompletionTimeOfDesc", parameters);

            TimeSpan compTime = TimeSpan.FromSeconds(Convert.ToDouble(outParam.Value));

            return compTime;
        }

        public async Task<Tree<Models.TaskWithSubtreePath>> GetFullTaskSubTree(int taskid)
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

        public async Task<Tree<TaskWithSubtreePath>> GetTreeWithParentsOfTask(int taskid)
        {
            var tasksWithPath = await _context.taskWithSubtreePaths.FromSql($"call getAllTaskParents({taskid})").ToListAsync();

            var headTask = tasksWithPath.FirstOrDefault(t => t.ParentTaskId == null);

            var headNode = new Node<Models.TaskWithSubtreePath>();

            headNode.Value = headTask;

            headNode.SubNodes = getAllDescendants(headNode, tasksWithPath);

            return new Tree<Models.TaskWithSubtreePath>()
            {
                HeadNode = headNode
            };
        }

        public IQueryable<Models.Task> GetTerminalTasksOfUser(string userid)
        {
            var tasks = _context.Tasks.Where(t => t.ParentTaskId == null && t.UserId == userid);

            return tasks;
        }

        public async Task<bool> HasUserTask(int taskid, string userid)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == taskid && t.UserId == userid);
        }
        
    }
}
