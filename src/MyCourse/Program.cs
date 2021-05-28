using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyCourse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webHostBuilder => {
                    webHostBuilder.UseStartup<Startup>();
                })

                //Se volessi configurare la DI in un'applicazione console userei:
                //.ConfigureServices

                //Posso ridefinire l'elenco dei provider di default
                /*.ConfigureLogging((context, builder) => {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.Add...;
                })*/

                //Posso ridefinire l'elenco delle fonti di configurazione con ConfigureAppConfiguration
                /*.ConfigureAppConfiguration((context, builder) => {
                    builder.Sources.Clear();
                    builder.AddJsonFile("appsettings.json", optional:true, reloadOnChange: true);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    //Qui altre fonti...
                })*/
                ;
    }
}
