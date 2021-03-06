namespace Demo.WebApi.Patch.Controllers.v2
{
    using Demo.WebApi.Patch.API.Models;    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;       
    
    [ApiController]
    [Route("api/[controller]")]
    // [Route("api/{version:apiVersion}/users")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private static readonly User[] UserRepo = new[]
        {
            new User { Id = 1, Name = "(V2) Test User 21", Email = "test21@email.com", Phone = "214-000-0000" },
            new User { Id = 2, Name = "(V2) Test User 22", Email = "test22@email.com", Phone = "972-000-0000" },
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == default) 
            { 
                return this.BadRequest();
            }

            var user = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (user == null) 
            {
                return this.NotFound($"No record with id {id} found in the system.");
            }

            return this.Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] User user) 
        {
            if (!ModelState.IsValid) 
            {
                return this.BadRequest(ModelState);
            }

            // Faking async call :(
            User existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return this.NotFound($"No record with id {id} found in the system.");
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Example Payload:
        /// 
        /// {  "ClientId": "123456",
        ///    "JsonPatchDocument": 
        ///    [
        ///            {
        ///              "value": "test789@email.com",
        ///              "OperationType": 2,
        ///              "path": "/Email",
        ///              "op": "replace",
        ///              "from": null
        ///            }
        ///    ]
        /// }
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] CustomJsonPatchDocument<User> patchDoc)
        {
            if (patchDoc is null || patchDoc.JsonPatchDocument is null)
            {
                return BadRequest($"{nameof(patchDoc)} patch object cannot be null");
            }

            // Faking async call :(
            User existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return this.NotFound($"No record with id {id} found in the system.");
            }

            // ApplyTo not validating model, so IsValid always returns "true"
            // However, if we call "TryValidateModel", then IsValid return correct result.
            // ToDo: Check "ApplyTo" source code. https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonPatchExtensions.cs
            patchDoc.JsonPatchDocument.ApplyTo(existingUser, ModelState);

            // Not working.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This works
            if (!TryValidateModel(existingUser))
            {
                return ValidationProblem(ModelState);
            }
            
            Console.WriteLine(JsonSerializer.Serialize(existingUser));

            return NoContent();
        }
    }
}
