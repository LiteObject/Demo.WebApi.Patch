using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.API.Binders
{
    public class CustomJsonPatchBinder : IModelBinder
    {
        private readonly Dictionary<Type, (ModelMetadata, IModelBinder)> binders;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            string modelName = bindingContext.ModelName;

            //Get command model payload (JSON) from the body  
            string valueFromBody;
            using (StreamReader streamReader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                valueFromBody = streamReader.ReadToEnd();
            }

            //Deserilaize body content to model instance  
            Type modelType = bindingContext.ModelMetadata.UnderlyingOrModelType;
            object modelInstance = JsonConvert.DeserializeObject(valueFromBody, modelType);

            bindingContext.Result = ModelBindingResult.Success(modelInstance);
            return Task.CompletedTask;
        }
    }
}
