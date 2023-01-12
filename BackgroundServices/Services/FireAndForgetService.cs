namespace BackgroundServices.Services
{
    public class FireAndForgetService : BackgroundService
    {
        private readonly ILogger<FireAndForgetService> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public FireAndForgetService(ILogger<FireAndForgetService> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var counter = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("FireAndForgetService is working. Count: {Count}", ++counter);
                await Task.Delay(1000, stoppingToken);
                await StopAsync(stoppingToken);
            }
        }
    }
}
