
using EarnVidhiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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


            var todayCompletedTasks = await _context.TaskHistories
            .Where(th => th.UserId == uid && th.CreatedAt.Value.Date == DateTime.Today)
            .ToListAsync();
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


    }
}
