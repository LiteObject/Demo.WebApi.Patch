using Demo.WebApi.Patch.API.Binders;
using Demo.WebApi.Patch.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly User[] UserRepo = new[]
        {
            new User { Id = 1, Name = "Test User", Email = "test@email.com" }
        };

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            User[] users = await Task.FromResult(UserRepo);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            User user = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (user == null)
            {
                return NotFound($"No record found with id {id}.");
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User existingUser = UserRepo.FirstOrDefault(u => u.Id == id);

            if (existingUser is null)
            {
                return BadRequest($"User not available in the system.");
            }

            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] CustomJsonPatchDocument<User> patchDoc)
        {
            if (patchDoc is null || patchDoc.JsonPatchDocument is null)
            {
                return BadRequest($"{nameof(patchDoc)} patch object cannot be null");
            }

            User existingUser = UserRepo.FirstOrDefault(u => u.Id == id);

            if (existingUser is null)
            {
                return BadRequest($"User not available in the system.");
            }

            if (Request.Headers.TryGetValue("x-application-id", out Microsoft.Extensions.Primitives.StringValues appid))
            {
                Console.WriteLine($"appid: {appid}");
            }

            if (Request.Headers.TryGetValue("x-username", out Microsoft.Extensions.Primitives.StringValues username))
            {
                Console.WriteLine($"username: {username}");
            }

            patchDoc.JsonPatchDocument.ApplyTo(existingUser, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine(JsonSerializer.Serialize(existingUser));

            return NoContent();
        }
    }
}
