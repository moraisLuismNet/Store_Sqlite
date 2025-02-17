using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Models;

namespace Store.Services
{
    public class ScheduledTaskService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly string fileName = "Discontinued Products.txt";

        public ScheduledTaskService(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _env = env;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

                    // Count discontinued products (Discontinued = true)
                    int quantityDiscontinued = await context.Products.CountAsync(p => p.Discontinued);

                    // Write to file
                    Write($"Discontinued Products: {quantityDiscontinued}");
                }
            }
            catch (Exception ex)
            {
                Write($"Error counting discontinued products: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void Write(string message)
        {
            var route = $@"{_env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
