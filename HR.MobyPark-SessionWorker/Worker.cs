using MySql.Data.MySqlClient;

namespace HR.MobyPark_SessionWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("HR.MobyPark Session Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Starting session cleanup task...");

                logger.LogInformation("Retrieving all user sessions from database...");
                List<UserSession> ActiveUserSessions = UserSession.GetAll();
                logger.LogInformation($"Retrieved {ActiveUserSessions.Count} user sessions from database.");

                logger.LogInformation("Retrieving UserActivity records from database...");
                List<UserActivity> UserActivities = UserActivity.GetAll();
                logger.LogInformation($"Retrieved {UserActivities.Count} UserActivity records from database.");

                logger.LogInformation("Filtering sessions...");
                List<Dictionary<UserSession, UserActivity>> ActiveSessionWithLatestActivity = new();
                foreach (UserActivity activity in UserActivities)
                {
                    foreach (UserSession session in ActiveUserSessions)
                    {
                        //Check to see if the user session matches the activity
                        if (activity.UserID == session.UserID && activity.Token == session.Token)
                        {
                            //If so check to see if we already have this session recorded
                            bool SessionAlreadyRecorded = false;
                            foreach (var dict in ActiveSessionWithLatestActivity)
                            {
                                if (dict.ContainsKey(session))
                                {
                                    SessionAlreadyRecorded = true;
                                    //Check to see if this activity is more recent than the recorded one
                                    UserActivity recordedActivity = dict[session];
                                    if (activity.ID > recordedActivity.ID)
                                    {
                                        //If so update the recorded activity
                                        dict[session] = activity;
                                    }
                                    break;
                                }
                            }
                            //If we don't have this session recorded yet, add it
                            if (!SessionAlreadyRecorded)
                            {
                                ActiveSessionWithLatestActivity.Add(new Dictionary<UserSession, UserActivity> { { session, activity } });
                            }
                        }
                    }
                }

                logger.LogInformation($"Filtered down to {ActiveSessionWithLatestActivity.Count} active sessions with latest activity.");
                logger.LogInformation("Checking for expired sessions...");

                List<UserSession> SessionsToDelete = new();
                DateTime CurrentTime = DateTime.Now;
                foreach (Dictionary<UserSession, UserActivity> dict in ActiveSessionWithLatestActivity)
                {
                    foreach (KeyValuePair<UserSession, UserActivity> currentActivitySession in dict)
                    {
                        UserSession session = currentActivitySession.Key;
                        UserActivity activity = currentActivitySession.Value;
                        TimeSpan InactivityDuration = CurrentTime - activity.CreateDateTime;
                        if (InactivityDuration.TotalMinutes > 60) //Assuming 60 minutes of inactivity means session is expired
                        {
                            SessionsToDelete.Add(session);
                            logger.LogInformation($"Session ID: {session.ID}, UserID: {session.UserID} marked for deletion due to inactivity of {InactivityDuration.TotalMinutes} minutes.");
                        }
                    }
                }

                logger.LogInformation($"Found {SessionsToDelete.Count} expired sessions to delete.");
                foreach (UserSession session in SessionsToDelete)
                {
                    if (session.DeleteDB())
                    {
                        logger.LogInformation($"Deleted expired session ID: {session.ID}, UserID: {session.UserID}");
                    }
                    else
                    {
                        logger.LogError($"Failed to delete expired session ID: {session.ID}, UserID: {session.UserID}");
                    }
                }

                logger.LogInformation($"Session cleanup task completed. task completed at: {DateTimeOffset.Now}, total time taken: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
