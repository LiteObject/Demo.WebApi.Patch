namespace Demo.WebApi.Patch.Controllers.v1
{
    using Demo.WebApi.Patch.API.Models;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// The v1 users controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    public class UsersController : ControllerBase
    {
        private static readonly User[] UserRepo = new[]
        {
            new User { Id = 1, Name = "(V1) Test User 11", Email = "test11@email.com" },
            new User { Id = 2, Name = "(V1) Test User 12", Email = "test12@email.com" },
        };

        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users
        ///     GET /api/v1/users
        ///
        /// </remarks>
        /// <returns>A collection of users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            User[] users = await Task.FromResult(UserRepo);

            if (users.Length == 0) 
            { 
                return NotFound();
            }

            return this.Ok(users);
        }

        /// <summary>
        /// Retrieves a user for the given id
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/{id}
        ///     GET /api/v1/users/{id}
        ///
        /// </remarks>
        /// <returns>A user</returns>
        /// <response code="200">If requested user is found</response>        
        /// <response code="404">If requested user is not found</response>
        /// <response code="400">If user id is invalid</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Math.Sign(Double.NaN);

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

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <param name="user">The updated user object</param>
        /// <returns>The updated user</returns>
        /// <response code="200">If requested user is found</response>        
        /// <response code="404">If requested user is not found</response>
        /// <response code="400">If request is invalid</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            // Faking async call :(
            User? existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return this.NotFound($"No record with id {id} found in the system.");
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Partially updates a user 
        /// </summary>
        /// <remarks> 
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
        /// </remarks>
        /// <param name="id">The resource identifier</param>
        /// <param name="patchDoc">The json patch document</param>
        /// <returns>No content</returns>
        /// <response code="204">If operation is successful</response>        
        /// <response code="404">If requested resource is not found</response>
        /// <response code="400">If request is invalid</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest($"{nameof(patchDoc)} patch object cannot be null");
            }

            // Faking async call :(
            User? existingUser = await Task.FromResult(UserRepo.FirstOrDefault(u => u.Id == id));

            if (existingUser is null)
            {
                return this.NotFound($"No record with id {id} found in the system.");
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
