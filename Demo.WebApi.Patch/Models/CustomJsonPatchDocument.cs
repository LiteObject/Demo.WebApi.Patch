using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.API.Models
{
    public class CustomJsonPatchDocument<T> where T : class
    {
        public int ClientId {  get; set; }

        public JsonPatchDocument<T> JsonPatchDocument {  get; set; }
    }
}
