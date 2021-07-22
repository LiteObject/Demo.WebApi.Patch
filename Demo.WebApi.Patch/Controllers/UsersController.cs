using Demo.WebApi.Patch.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            var users = await Task.FromResult(UserRepo);
            return this.Ok(users);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<User> patchDoc) 
        {
            if (patchDoc is null)
            {
                return BadRequest($"{nameof(patchDoc)} patch object cannot be null");
            }

            var existingUser = UserRepo.FirstOrDefault(u => u.Id == id);

            if (existingUser is null) 
            {
                return BadRequest($"User not available in the system.");
            }

            patchDoc.ApplyTo(existingUser, ModelState);

            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine(JsonSerializer.Serialize(existingUser));

            return NoContent();
        }
    }
}
