namespace Demo.WebApi.Patch.API.Controllers.V2
{
    using Demo.WebApi.Patch.API.Models;
    using Demo.WebApi.Patch.API.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await Task.FromResult(UserService.GetAll());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            var user = await Task.FromResult(UserService.GetById(id));

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

            if (id != user.Id)
            {
                return BadRequest();
            }

            // Faking async call :(
            User existingUser = await Task.FromResult(UserService.GetById(id));

            if (existingUser is null)
            {
                return NotFound($"No record with id {id} found in the system.");
            }

            UserService.Update(user);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingUser = UserService.GetById(id);

            if (existingUser is null)
            {
                return NotFound();
            }

            // Delete user here
            UserService.Delete(id);

            return NoContent();
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
            User existingUser = await Task.FromResult(UserService.GetById(id));

            if (existingUser is null)
            {
                return NotFound($"No record with id {id} found in the system.");
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
