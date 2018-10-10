using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TestTask.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (context.Database.EnsureCreated())
            {
                context.Database.ExecuteSqlCommand(
                $"CREATE PROCEDURE getPlannedLabOfDesc(in taskid int(11), out result int(11)) " +
                $"BEGIN " +
                    $"WITH recursive task_path (id, PlannedLaboriousness) as ( " +
                    $"select id, PlannedLaboriousness from tasks " +
                    $"where id = taskid " +
                    $"union all " +
                    $"select t.id, t.PlannedLaboriousness " +
                    $"from tasks as t join task_path as tp " +
                    $"on tp.id = t.parenttaskid ) SELECT sum(PlannedLaboriousness) from task_path " +
                    $"where id != taskid INTO result; " +
                $"END");
                context.Database.ExecuteSqlCommand(
                        $"CREATE PROCEDURE getCompletionTimeOfDesc(in taskid int(11), out result time(6)) " +
                        $"BEGIN " +
                            $"WITH recursive task_path (id, CompletionTimeInSec) as ( " +
                            $"select id, time_to_sec(CompletionTime) as CompletionTimeInSec from tasks " +
                            $"where id = taskid " +
                            $"union all " +
                            $"select t.id, time_to_sec(t.CompletionTime) as CompletionTimeInSec " +
                            $"from tasks as t join task_path as tp " +
                            $"on tp.id = t.parenttaskid ) SELECT sec_to_time(sum(CompletionTimeInSec)) from task_path " +
                            $"where id != taskid into result; " +
                        $"END");
                context.Database.ExecuteSqlCommand(
                        $"create procedure getAllTaskDescendants(in taskid int(11)) " +
                        $"BEGIN " +
                            $"WITH recursive task_path as ( " +
                            $"select id, name, Description, Performers, CreationDate, Status, PlannedLaboriousness, " +
                            $"CompletionTime, ActualCompletionDate, UserId, CAST(id AS CHAR(200)) as path from tasks " +
                            $"where id = taskid " +
                            $"union all " +
                            $"select t.id, t.name, t.Description, t.Performers, t.CreationDate, t.Status, t.PlannedLaboriousness, " +
                            $"t.CompletionTime, t.ActualCompletionDate, t.UserId, concat(tp.path,',',t.id) as path " +
                            $"from tasks as t join task_path as tp " +
                            $"on tp.id = t.parenttaskid ) SELECT * from task_path; " +
                        $"END");
            }
            
            List<User> users;

            if (!context.Users.Any())
            {
                users = new List<User>();

                for (int i = 1; i < 10; i++)
                {
                    var user = new User()
                    {
                        UserName = $"TestUser{i}",
                        Email = $"TestUser@qwer{i}.com"
                    };

                    users.Add(user);

                    var result = userManager.CreateAsync(user, $"UserPassword@{i}").Result;

                    if (!result.Succeeded)
                    { 
                        foreach(var error in result.Errors)
                        {
                            throw new Exception(error.Description);
                        }
                    }
                }
            } else
            {
                users = context.Users.FromSql($"SELECT * FROM aspnetusers" +
                                              $" WHERE username LIKE '%TestUser%';").ToList();

            }

            if (!context.Tasks.Any())
            {
                var tasks = new List<Models.Task>();

                foreach(var user in users)
                {
                    var termTask = new Models.Task()
                    {
                        User = user,
                        Name = $"{user.UserName}'s Terminal Task 1",
                        Description = "Lorem lorem input lorem asdsasd \nLOREM INPUT!!!!!",
                        Performers = "Vanya Rudnov",
                        PlannedLaboriousness = 16,
                        CompletionTime = new TimeSpan(16, 45, 0)
                    };

                    tasks.Add(termTask);
                    
                    for (int i = 1; i <= 3; i++)
                    {
                        var task = new Models.Task()
                        {
                            User = user,
                            ParentTask = termTask,
                            Name = $"{user.UserName}'s sub task {i}",
                            Description = "Lorem lorem input lorem asdsasd \nLOREM INPUT!!!!!",
                            Performers = "Vanya Rudnov",
                            PlannedLaboriousness = 16,
                            CompletionTime = new TimeSpan(16, 45, 0)
                        };

                        tasks.Add(new Models.Task()
                        {
                            User = user,
                            ParentTask = task,
                            Name = $"{user.UserName}'s sub sub task {i}",
                            Description = "Lorem lorem input lorem asdsasd \nLOREM INPUT!!!!!",
                            Performers = "Vanya Rudnov",
                            PlannedLaboriousness = 16,
                            CompletionTime = new TimeSpan(16, 45, 0)
                        });
                    }

                }

                context.AddRange(tasks);

                context.SaveChanges();

            }
        }
    }
}
