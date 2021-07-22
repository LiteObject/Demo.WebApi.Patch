using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.ConsoleApp
{
    interface IUserService
    {
        [Patch("/api/users/{id}")]
        public Task<HttpResponseMessage> PatchUserAsync(int id, [Body] PatchPayload[] patchDoc);
    }
}
