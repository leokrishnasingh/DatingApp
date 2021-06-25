using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(int id)
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet]
        [Route("GetUsers/{id}")]
        public async Task<ActionResult<AppUser>>  GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id==id);
            return user;
        }
    }
}