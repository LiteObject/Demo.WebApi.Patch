using Microsoft.AspNetCore.JsonPatch;

namespace Demo.WebApi.Patch.API.Models
{
    /// <summary>
    /// The custom patch doc
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomJsonPatchDocument<T> where T : class
    {
        /// <summary>
        /// The client identifier
        /// </summary>
        public int ClientId {  get; set; }

        /// <summary>
        /// The JsonPatchDocument
        /// </summary>
        public JsonPatchDocument<T> JsonPatchDocument {  get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="jsonPatchDocument"></param>
        public CustomJsonPatchDocument(int clientId, JsonPatchDocument<T> jsonPatchDocument)
        {
            ClientId = clientId;
            JsonPatchDocument = jsonPatchDocument;
        }
    }
}
