using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context,ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registeDTO)
        {
            if(await UserExists(registeDTO.Username)) return BadRequest("Username taken");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName=registeDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registeDTO.Password)),
                PasswordSalt=hmac.Key
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDTO{Username=user.UserName,Token=_tokenService.CreateToken(user)}; 
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName==username.ToLower());
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => 
                        x.UserName== loginDTO.Username.ToLower());

            if(user == null) return Unauthorized("Invalid Username");

            else{
                using var hmac = new  HMACSHA512(user.PasswordSalt);
                
                var computeHashCode = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

                for(int i=0;i<computeHashCode.Length;i++){
                    if(computeHashCode[i]!=user.PasswordHash[i]) return Unauthorized("Incorrect Password");
                }

                return new UserDTO{Username=user.UserName,Token=_tokenService.CreateToken(user)}; 
            }
        }
    }
}