using Demo.WebApi.Patch.API.Swagger;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Reflection;

namespace Demo.WebApi.Patch
{
    /// <summary>
    /// Source: https://github.com/dotnet/aspnet-api-versioning/tree/master/samples 
    /// Source: https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-5.0
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            /*
            * AddNewtonsoftJson replaces the System.Text.Json-based input and output formatters used 
            * for formatting all JSON content. To add support for JSON Patch using Newtonsoft.Json, 
            * while leaving the other formatters unchanged, update the project's Startup.ConfigureServices 
            * method as follows:
            */
            _ = builder.Services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddNewtonsoftJson();

            _ = builder.Services.AddProblemDetails();

            _ = builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            _ = builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = ApiVersion.Default; // = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;

                // Add the headers: "api-supported-versions" and "api-deprecated-versions" to let the clients of the API know all supported versions
                config.ReportApiVersions = true;

                config.ApiVersionReader = ApiVersionReader.Combine(
                    // new HeaderApiVersionReader("api-version"),
                    // new QueryStringApiVersionReader("api-version"),
                    // To send version info in the accept header ("accrpt" header advertises which content types client is able to understand)
                    new MediaTypeApiVersionReader("api-version")
                );
            });

            // To add an API explorer that is API version aware
            _ = builder.Services.AddVersionedApiExplorer(options =>
            {
                // Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // NOTE: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            _ = builder.Services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo.WebApi.Patch.API", Version = "v1" });
                // c.DocumentFilter<JsonPatchDocumentFilter>();

                c.OperationFilter<SwaggerDefaultValues>();
                c.EnableAnnotations();

                // Generate the XML docs for Swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }

            _ = app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(
                               async (context) =>
                               {
                                   var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                                   if (exceptionHandlerFeature != null)
                                   {
                                       // var logger = LoggerFactory.CreateLogger("Global Exception Logger");
                                       app.Logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                                   }

                                   // Response to client
                                   context.Response.StatusCode = 500;
                                   await context.Response.WriteAsync("Encountered an unexpected fault. Try again later.");
                               });
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                // c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
