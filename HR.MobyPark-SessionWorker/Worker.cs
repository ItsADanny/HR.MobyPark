namespace HR.MobyPark_SessionWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                DateTime startMoment = DateTime.Now;
                logger.LogInformation("HR.MobyPark Session Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Retrieving open usersessions");
                List<UserSession> openUserSession = UserSession.GetAll(Program.DBHelper);

                logger.LogInformation("Open usersessions retrieved, retrieving per session activities for comparison");
                foreach (UserSession userSession in openUserSession)
                {
                    List<UserActivity> userActivityForSession = UserActivity.GetAll(Program.DBHelper, userSession.Token);

                    List<UserActivity> CurrentActivities = (from userAct in userActivityForSession
                                                           where userAct.CreateDateTime >= DateTime.Today.AddHours(DateTime.Now.Hour - 1)
                                                           select userAct).ToList();

                    if (CurrentActivities.Count() >= 1) openUserSession.Remove(userSession);
                }
                logger.LogInformation($"Open sessions filter, Inactive session count: {openUserSession.Count()}\n start closing sessions");
                foreach (UserSession userSession in openUserSession)
                {
                    logger.LogInformation($"Session for UserID: {userSession.UserID} with token: {userSession.Token} has been closed");
                    userSession.DeleteDB(Program.DBHelper);
                } 
            }
            await Task.Delay(Converter.DayToMilisecond(1), stoppingToken);
        }
    }
}
