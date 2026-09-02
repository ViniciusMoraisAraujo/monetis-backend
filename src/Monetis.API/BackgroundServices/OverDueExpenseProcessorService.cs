using Monetis.Application.Abstractions.Services;

namespace Monetis.API.BackgroundServices;

public class OverDueExpenseProcessorService(
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddMinutes(1);
                var delay = nextRun - now;

                if (delay <= TimeSpan.Zero)
                {
                    delay = TimeSpan.FromMinutes(1);
                }


                await Task.Delay(delay, stoppingToken);

                using (var scope = serviceProvider.CreateScope())
                {
                    var expenseService = scope.ServiceProvider.GetRequiredService<IExpenseService>();

                    await expenseService.ProcessOverdueExpensesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
