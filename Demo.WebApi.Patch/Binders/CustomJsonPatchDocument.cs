using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Demo.WebApi.Patch.API.Binders
{
    public class CustomJsonPatchDocument<T> 
        where T : class
{
        [FromBody]
        public string ClientId { get; set; }

        [FromBody]
        public JsonPatchDocument<T> JsonPatchDocument { get; set; }
    }
}
