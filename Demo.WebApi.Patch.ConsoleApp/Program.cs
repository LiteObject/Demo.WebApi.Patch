using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Threading.Tasks;

namespace Demo.WebApi.Patch.ConsoleApp
{
    class Program
    {
        private static readonly ServiceProvider Provider;

        private static IConfiguration configuration;

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

            PatchPayload[] userPatchRequest = new PatchPayload[] 
            {
                 new PatchPayload()
                    {
                        Path = "/Email",
                        Value = "test-456@email.com",
                        Op = "replace"
                    }
            };

            try
            {
                using var httpResponseMessage = await service.PatchUserAsync(1, userPatchRequest);

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
                /*.AddJsonFile($"appsettings.{environmentName}.json", true, true) */
                .AddEnvironmentVariables();

            services.AddRefitClient<IUserService>(new RefitSettings()
            {
                // To use "System.Text.Json"
                // ContentSerializer = new RefitJsonContentSerializer()
            }).ConfigureHttpClient(
                c =>
                {
                    c.BaseAddress = new Uri("http://localhost:5000");
                });
        }
    }
}
