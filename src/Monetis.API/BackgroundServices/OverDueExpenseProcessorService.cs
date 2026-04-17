using Monetis.Application.Interfaces;

namespace Monetis.API.BackgroundServices;

public class OverdueExpenseProcessorService(
    IServiceProvider serviceProvider,
    ILogger<OverdueExpenseProcessorService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Overdue Expense Processor started");

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
                
                logger.LogInformation("Next overdue check scheduled for: {NextRun}", nextRun);

                await Task.Delay(delay, stoppingToken);

                using (var scope = serviceProvider.CreateScope())
                {
                    var expenseService = scope.ServiceProvider.GetRequiredService<IExpenseService>();
                    
                    logger.LogInformation("Processing overdue expenses...");
                    await expenseService.ProcessOverdueExpensesAsync(stoppingToken);
                    logger.LogInformation("Overdue expenses processed successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing overdue expenses");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}