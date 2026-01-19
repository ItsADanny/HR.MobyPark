namespace HR.MobyPark_BillingUsersWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("HR.MobyPark Billing Worker running at: {time}", DateTimeOffset.Now);
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

                Email email = new Email("", "Unpaid Billings Overview Report", "Weekly overview report of unpaid billings per user:<br>");
                
                string tableHtml = "<table border='1'><tr><th>User ID</th><th>User Email</th><th>Number of Unpaid Billings</th><th>Total Amount Due</th></tr>";
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
                    decimal totalAmountDue = userBillings.Sum(b => b.Amount);
                    tableHtml += $"<tr><td>{userID}</td><td>{user.Email}</td><td>{userBillings.Count}</td><td>{totalAmountDue:C}</td></tr>";
                }
                tableHtml += "</table>";
                email.Body += tableHtml;
                email.SendMail();
                logger.LogInformation("Sent unpaid billings overview report email.");

                logger.LogInformation($"Unpaid billings overview report task completed, task completed at: {DateTimeOffset.Now}, total time taken: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
