using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.ConsoleApp
{
    class Program
    {
        private static readonly ServiceProvider Provider;

        static Program()
        {
            var serviceCollection = new ServiceCollection();
            string[] arguments = Environment.GetCommandLineArgs();
            Configure(serviceCollection);

            Provider = serviceCollection.BuildServiceProvider();
        }

        static async Task Main(string[] args)
        {
            var service = Provider.GetRequiredService<IUserService>();

            // This can be used as well instead of "PatchPayload"
            JsonPatchDocument<User> patchDoc = new JsonPatchDocument<User>();
            patchDoc.Replace(x => x.Email, "test789@email.com");
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(patchDoc.Operations);

            /*PatchPayload[] userPatchRequest = new PatchPayload[] 
            {
                 new PatchPayload()
                    {
                        Path = "/Email",
                        Value = "test-456@email.com",
                        Op = "replace"
                    }
            }; */

            try
            {                
                //using var httpResponseMessage = await service.PatchUserAsync(1, userPatchRequest, "mhoque");
                using var httpResponseMessage = await service.PatchUserAsync(1, new StringContent(jsonPayload, System.Text.Encoding.Unicode, MediaTypeNames.Application.Json));

                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Console.WriteLine("SUCCESSFUL");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private static void Configure(IServiceCollection services)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)                
                .AddEnvironmentVariables();

            services.AddSingleton<LoggingHandler>();

            services.AddRefitClient<IUserService>().ConfigureHttpClient(
                c =>
                {
                    c.BaseAddress = new Uri("http://localhost:5000");
                    c.DefaultRequestHeaders.Add("x-application-id", "my-test-app");
                }).AddHttpMessageHandler<LoggingHandler>();
        }
    }
}
