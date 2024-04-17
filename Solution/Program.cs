using Serilog;
using Serilog.Events;

namespace Solution;

internal static class Program
{
    public static async Task Main()
    {
        // Configure serilog logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("../../../../var/log_error.txt", LogEventLevel.Error)
            .WriteTo.File("../../../../var/log_information.txt", LogEventLevel.Information)
            .CreateLogger();

        // Create and start bot
        using var cts = new CancellationTokenSource();
        var telegramBot = new TelegramBot();
        await telegramBot.StartAsync(cts.Token);
    }
}