using Azure;
using EarnVidhiCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EarnVidhiCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
    
        public IConfiguration _configuration;
        public readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthController(IConfiguration config, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = config;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("signup/")]
        public async Task<ActionResult<User>> SignUp(User user)
        {
            dynamic response = new ExpandoObject();
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    errors.Add("Name field is Required");
                }

                if (string.IsNullOrWhiteSpace(user.UserMobile))
                {
                    errors.Add("Mobile Field is Required");
                }

                if (string.IsNullOrWhiteSpace(user.UserEmail))
                {
                    errors.Add("Email Field is Required");
                }
                else if (!user.UserEmail.Contains('@'))
                {
                    errors.Add("Invalid Email Format");
                }

                if (string.IsNullOrWhiteSpace(user.UserPassword))
                {
                    errors.Add("Password Field is Required");
                }

                var CheckUser = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == user.UserEmail || x.UserMobile == user.UserMobile);
                if (CheckUser != null)
                {
                    errors.Add("user already exist with same email..!");
                }

                if (errors.Count > 0)
                {
                    response.status = 0;
                    response.msg = "Required fields are missing";
                    response.error = errors;
                    return Ok(response);
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                //send mail

                var verifyLink = "";

                new MailLogic(_configuration, _httpContextAccessor).SendOtpMail(user.UserEmail, verifyLink);

                response.status = 1;
                response.msg = "User Register Successfully now verify Your email!";
                response.data = user;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.status = 2;
                response.msg = "Some issue accured";
                response.error = ex;
                return Ok(response);
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(User loguser)
        {
            dynamic response = new ExpandoObject();
            if (loguser.UserEmail != null && loguser.UserPassword != null)
            {

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == loguser.UserEmail || x.UserPassword == loguser.UserPassword);

                if (user != null)
                {



                    var verifyCode = GenerateCode();
                    var verify = new EmailVerify() { UserId = user.UserId, VerifyCode = verifyCode };
                    await _context.EmailVerify.AddAsync(verify);
                    await _context.SaveChangesAsync();

                    new MailLogic(_configuration, _httpContextAccessor).SendOtpMail(verifyCode, user.UserEmail);



                    if (user.UserStatus == "block")
                    {
                        response.status = 0;
                        response.msg = "Account blocked cant login!";
                        return Ok(response);
                    }
                    if (user.UserEmailVerify != 1)
                    {
                        response.status = 0;
                        response.msg = "Please verify Your email!";
                        return Ok(response);
                    }
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("UserName", user.UserName),
                        new Claim("UserEmail", user.UserEmail)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
                    response.status = 1;
                    response.msg = "success";
                    response.token = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(response);
                }
                else
                {
                    response.status = 0;
                    response.msg = "Invalid credentials";
                    return Ok(response);
                }
            }
            else
            {
                response.status = 0;
                response.msg = "Invalid credentials";
                return Ok(response);
            }
        }

        [NonAction]
        public string GenerateCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string otp = new string(new char[20].Select(_ => chars[random.Next(chars.Length)]).ToArray());
            return otp;
        }
    }
}
