namespace HR.MobyPark_DashboardingDBWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private static string LogDBStartedTableString(string table) => $"Started copying table: {table}";
    private static string LogDBFinishedTableString(string table, int total) => $"Finished copying table: {table}, Total rows copied/updated: {total}";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("HR.MobyPark Dashboarding Database Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Started clearing database");
                Program.DBH_ToDB.Delete(ParkingLot.ToDeleteAllSQL());
                logger.LogInformation("");
            }
            await Task.Delay((int) Converter.HourToMilisecond(24), stoppingToken);
        }
    }
}
