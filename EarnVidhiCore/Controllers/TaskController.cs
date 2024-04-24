﻿
using Azure;
using EarnVidhiCore.Dtos;
using EarnVidhiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EarnVidhiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        public IConfiguration _configuration;
        readonly IHttpContextAccessor _httpcontext;
        public readonly ApplicationDbContext _context;
        public TaskController(IConfiguration config, IHttpContextAccessor httpcontext, ApplicationDbContext context)
        {
            _configuration = config;
            _httpcontext = httpcontext;
            _context = context;
        }

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {            
            dynamic response = new ExpandoObject();
            int uid = Convert.ToInt32(_httpcontext.HttpContext.User.Claims
                       .First(i => i.Type == "UserId").Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == uid);
            var todayCompletedTasks = await _context.TaskHistories.Where(th => th.UserId == uid && th.CreatedAt.Value.Date == DateTime.Today).ToListAsync();
            decimal walletAmount = user.MainWallet ?? 0;
            var data = new
            {
                User = user,
                TodayCompletedTasks = todayCompletedTasks.Count,
                WalletAmount = walletAmount
            };
            response.status = 1;
            response.msg = "success";
            response.data = data;
            return Ok(response);
        }

        [HttpGet("allTasks")]
        [Authorize]
        public async Task<IActionResult> AllTasks()
        {
            dynamic response = new ExpandoObject();
            try
            {
                int UId = getuserid();
                User? user = _context.Users.FirstOrDefault(x => x.UserId == UId);
                if (user == null)
                {
                    response.status = 0;
                    response.msg = "Not Allowed for Tasks";
                    return Ok(response);
                }

                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string token = new string(Enumerable.Repeat(chars, 20)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                token = UId + token;
                List<TasksDto> tasks = new List<TasksDto>();
                var todayTaskLogs = await _context.TaskHistories.Where(log => log.CreatedAt.HasValue && log.CreatedAt.Value.Date == DateTime.Today && log.UserId == UId && log.Status==1).ToListAsync();

                int i = 1;

                foreach (TaskHistory log in todayTaskLogs)
                {
                    TasksDto model = new TasksDto()
                    {
                        Id = i,
                        TaskName = $"Task {i}",
                        Url = $"{log.TaskId}",
                        IsCompleted = "Completed"
                    };
                    i++;
                    tasks.Add(model);
                }

                var prevTask = await _context.TaskHistories.FirstOrDefaultAsync(log => log.CreatedAt.HasValue && log.CreatedAt.Value.Date == DateTime.Today && log.UserId == UId && log.Status == 0);
                var taskRecord = await _context.Tasks.OrderBy(r => Guid.NewGuid()).FirstOrDefaultAsync(x=>x.TaskStatus==1);

                if (todayTaskLogs.Count < 9)
                {
                    if (prevTask != null)
                    {
                        var theTask = await _context.Tasks.FirstOrDefaultAsync(x => x.TaskId == prevTask.TaskId);
                        tasks.Add(new TasksDto
                        {
                            Id = i,
                            TaskName = $"Task {i++}",
                            Url = $"{theTask.TaskUrl}?&token={token}&User={user.UserPromo}",
                            IsCompleted = "Start"
                        });

                        prevTask.TaskToken = token;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TaskHistory taskHistory = new TaskHistory() { TaskId = taskRecord.TaskId,TaskToken=token,Status=0,UserId=UId,CreatedAt=DateTime.Now };
                        tasks.Add(new TasksDto
                        {
                            Id = i,
                            TaskName = $"Task {i++}",
                            Url = $"{taskRecord.TaskUrl}?&token={token}&User={user.UserPromo}",
                            IsCompleted = "Start"
                        });
                        await _context.TaskHistories.AddAsync(taskHistory);
                        await _context.SaveChangesAsync();
                    }
                }

                for (int x = i; x <= 9; x++)
                {
                    TasksDto model = new TasksDto();
                    model.Id = x;
                    model.Url = "Task" + x;
                    model.TaskName = "Task" + x;
                    model.IsCompleted = "Pending";
                    tasks.Add(model);
                }

                response.status = 1;
                response.msg = "success";
                response.data = tasks;
                return Ok(response);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [NonAction]
        public int getuserid()
        {
            return Convert.ToInt32(User.Claims.First().Value);
        }
    }
}
