using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Controllers
{
    [Authorize]
    [Route("Task")]
    public class TaskController : Controller
    {

        private readonly TaskService _taskService;

        private readonly UserManager<User> _userManager;

        public TaskController(TaskService taskService, UserManager<User> userManager)
        {
            _taskService = taskService;

            _userManager = userManager;
        }

        [HttpGet("/")]
        [HttpGet("{id:int?}")]
        public async Task<IActionResult> Index(int? id)
        {
            ViewData["taskid"] = id;

            var userid = _userManager.GetUserId(User);

            Models.Task task = null;

            if(id == null)
            {
                try
                {
                    task = await _taskService.GetTerminalTasksOfUser(userid)
                        .OrderBy(t => t.Name).FirstAsync();

                } catch(InvalidOperationException ex)
                { 

                    return RedirectToAction("create", "task");
                }
                
            } else
            {
                if (!await _taskService.HasUserTask((int) id, userid))
                {
                    return NotFound(); 
                }

                task = await _taskService.GetTaskById((int) id);

                ViewData["taskid"] = task.Id;
            }


            return View(task);
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["taskid"] = id;

            if(await _taskService.HasUserTask(id, _userManager.GetUserId(User)))
            {
                return View(await _taskService.GetTaskById(id));
            }

            return NotFound();
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Models.Task task)
        {
            if (ModelState.IsValid)
            {
                if (await _taskService.HasUserTask(task.Id, _userManager.GetUserId(User)))
                {
                    await _taskService.Edit(task);

                    return RedirectToAction("Index", "task", new { id = task.Id });
                }
            }
            return View(task);
        }

        [HttpGet("create/{id:int?}")]
        public async Task<IActionResult> Create(int? id)
        {
            ViewData["taskid"] = id;

            if(id == null)
            {
                return View(new Models.Task());
            }

            if(await _taskService.HasUserTask((int)id, _userManager.GetUserId(User)))
            {
                return View(new Models.Task() { ParentTaskId = id });
            }

            return NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Models.Task task)
        {
            if (ModelState.IsValid)
            {
                await _taskService.CreateTask(task, _userManager.GetUserId(User));

                return RedirectToAction("index", "task", new { id = task.Id });
            }
            return View(task);
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _taskService.HasUserTask(id, _userManager.GetUserId(User)))
            {
                await _taskService.DeleteTask(id);

                return Redirect("/task");
            }

            return NotFound();
        }

    }
}