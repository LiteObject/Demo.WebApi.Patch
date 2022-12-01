﻿namespace Demo.WebApi.Patch.API.Controllers.V1
{
    using Demo.WebApi.Patch.API.Models;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    public class Users1Controller : ControllerBase
    {
        private static readonly User[] UserRepo = new[]
        {
            new User { Id = 1, Name = "Test User 1", Email = "test1@email.com" },
            new User { Id = 2, Name = "Test User 2", Email = "test2@email.com" },
        };

        private readonly ILogger<Users1Controller> _logger;

        public Users1Controller(ILogger<Users1Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await Task.FromResult(UserRepo);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            var user = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (user == null)
            {
                return NotFound($"No record with id {id} found in the system.");
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

            // Faking async call :(
            User existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return NotFound($"No record with id {id} found in the system.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Example Payload:
        ///         
        ///    [
        ///            {
        ///              "value": "test789@email.com",
        ///              "OperationType": 2,
        ///              "path": "/Email",
        ///              "op": "replace",
        ///              "from": null
        ///            }
        ///    ]        
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest($"{nameof(patchDoc)} patch object cannot be null");
            }

            // Faking async call :(
            User existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return NotFound($"No record with id {id} found in the system.");
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