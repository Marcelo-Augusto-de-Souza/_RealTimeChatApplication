using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    // Configuração de CORS
                    services.AddCors(options =>
                    {
                        options.AddPolicy("AllowAll",
                            builder => builder
                                .AllowAnyOrigin()    // Permite qualquer origem
                                .AllowAnyMethod()    // Permite qualquer método (GET, POST, etc.)
                                .AllowAnyHeader());  // Permite qualquer cabeçalho
                    });

                    services.AddSingleton<ClientManager>(); // Register ClientManager as a singleton
                    services.AddHostedService<ClientMonitorService>(); // Register the background service if needed
                    services.AddControllers(); // Add controller support for API endpoints
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();

                    // Aplicando a política de CORS
                    app.UseCors("AllowAll");

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers(); // Map controllers to handle HTTP requests
                    });
                });

                webBuilder.UseUrls("http://0.0.0.0:5000/");
            });
}