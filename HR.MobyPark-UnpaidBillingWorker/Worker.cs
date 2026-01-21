namespace HR.MobyPark_UnpaidBillingWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("HR.MobyPark Unpaid Billing Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Retrieving all billings from database...");
                List<Billing> AllBillings = Billing.GetAll();
                logger.LogInformation($"Retrieved {AllBillings.Count} billings from database.");
                List<Billing> UnpaidBillings = AllBillings.Where(b => b.CompletedDateTime == null && b.CreateDateTime < DateTime.Now).ToList();
                logger.LogInformation($"Found {UnpaidBillings.Count} unpaid billings");

                logger.LogInformation("Retrieving all sessions from database...");
                List<Session> AllSessions = Session.GetAll();
                logger.LogInformation($"Retrieved {AllSessions.Count} sessions from database.");

                Dictionary<int, List<Billing>> billingsByUser = new();
                foreach (Billing billing in UnpaidBillings)
                {
                    Session? relatedSession = AllSessions.FirstOrDefault(s => s.ID == billing.SessionID);
                    if (relatedSession is null)
                    {
                        //This should not happen, but if it does, log a warning and skip this billing
                        logger.LogWarning($"No related session found for billing ID {billing.ID} with SessionID {billing.SessionID}");
                        continue;
                    }
                    else
                    {
                        if (!billingsByUser.ContainsKey(relatedSession.UserID))
                        {
                            billingsByUser[relatedSession.UserID] = new List<Billing>();
                        }
                        billingsByUser[relatedSession.UserID].Add(billing);
                    }
                }

                foreach (KeyValuePair<int, List<Billing>> entry in billingsByUser)
                {
                    int userID = entry.Key;
                    List<Billing> userBillings = entry.Value;

                    User? user = User.GetByID(userID);
                    if (user is null)
                    {
                        logger.LogWarning($"No user found with ID {userID}");
                        continue;
                    }

                    string emailBody = "Dear User,<br/><br/>The following billings are unpaid:<br/><ul>";
                    foreach (Billing billing in userBillings)
                    {
                        emailBody += $"<li>Billing ID: {billing.ID}, Amount: {billing.Amount}, Created On: {billing.CreateDateTime}</li>";
                    }
                    emailBody += "</ul><br/>Please make the necessary payments at your earliest convenience.<br/><br/>Best regards,<br/>MobyPark Team";

                    Email email = new Email(user.Email, "Unpaid Billings Notification", emailBody);
                    bool emailSent = email.SendMail();
                    if (emailSent)
                    {
                        logger.LogInformation($"Unpaid billing notification sent to user ID {userID} at {user.Email}");
                    }
                    else
                    {
                        logger.LogError($"Failed to send unpaid billing notification to user ID {userID} at {user.Email}");
                    }
                }

                logger.LogInformation($"Unpaid billing notification task completed, task completed at: {DateTimeOffset.Now}, total time taken: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");
            }
            await Task.Delay((int) Converter.DayToMilisecond(30), stoppingToken);
        }
    }
}
