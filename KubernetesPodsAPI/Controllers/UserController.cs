using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KubernetesPodsAPI.Data;
using KubernetesPodsAPI.Dtos;
using KubernetesPodsAPI.Models;
using KubernetesPodsAPI.Services;

namespace KubernetesPodsAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public UserController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Name)) return BadRequest("Name Is Already Taken");
            var hmac = new HMACSHA512();

            var user = new User
            {
                Login = registerDto.Name.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.Login,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string Name)
        {
            return await _context.Users.AnyAsync(x => x.Login == Name.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Login == loginDto.Username);

            if (user == null) return Unauthorized("Invalid Name");
            var hmaz = new HMACSHA512();
            var PasswordSalt = hmaz.Key;

            var hmac = new HMACSHA512(PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.Login,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
