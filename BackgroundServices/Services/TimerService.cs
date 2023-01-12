using Cronos;

namespace BackgroundServices.Services;

public class TimerService : BackgroundService
{
    private readonly ILogger<TimerService> _logger;
    private readonly string cronExp = "0/5 * * * * ?"; // 5 seconds
    private DateTime nextRunTime = DateTime.UtcNow;
    private int _counter = 0;
    public TimerService(ILogger<TimerService> logger)
    {
        _logger = logger;
        SetNextDate();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelayTime(), stoppingToken);
                _logger.LogInformation("TimerService is working. {0}", _counter++);
                nextRunTime = GetNextDate();
            }
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError(e, "Error");
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    private int GetDelayTime()
    {
        var delayTime = (nextRunTime - DateTime.UtcNow).TotalMilliseconds;
        if (delayTime < 0)
        {
            nextRunTime = GetNextDate();
            delayTime = (nextRunTime - DateTime.UtcNow).TotalMilliseconds;
        }
        return (int)delayTime;
    }
    private DateTime GetNextDate()
    {
        var cron = CronExpression.Parse(cronExp, CronFormat.IncludeSeconds);
        var next = cron.GetNextOccurrence(DateTime.UtcNow);
        return next ?? DateTime.UtcNow;
    }
    private void SetNextDate()
    {
        nextRunTime = GetNextDate();
    }

}

