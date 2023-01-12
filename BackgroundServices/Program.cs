using BackgroundServices.Services;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHostedService<FireAndForgetService>();
builder.Services.AddSingleton<TimerService>();
builder.Services.AddHostedService(service => service.GetRequiredService<TimerService>());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("api/startService", async (ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider) =>
{
    var logger =loggerFactory.CreateLogger("start");
    var fireLogger = loggerFactory.CreateLogger<FireAndForgetService>();
    logger.LogInformation("startsertvice");
    var fireService = new FireAndForgetService(fireLogger, applicationLifetime);
    await fireService.StartAsync(CancellationToken.None); 
    return "success";
});
app.MapGet("api/stopTimer", async (ILoggerFactory loggerFactory, IServiceProvider serviceProvider) =>
{
    var timer2Service = serviceProvider.GetRequiredService<TimerService>();
    await timer2Service.StopAsync(CancellationToken.None);
    return "success";
});
app.MapGet("api/startTimer", async (ILoggerFactory loggerFactory, IServiceProvider serviceProvider) =>
{
    var timerService = serviceProvider.GetRequiredService<TimerService>();
    await timerService.StartAsync(CancellationToken.None);
    return "success";
});

app.Run();
