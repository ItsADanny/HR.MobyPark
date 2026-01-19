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
                logger.LogInformation("Started clearing database...");
                Program.DBH_ToDB.Delete(ParkingLot.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Billing.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Payment.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Reservation.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Session.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(User.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserActivity.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserRole.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserSession.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Vehicle.ToDeleteAllSQL());
                logger.LogInformation("Finished clearing database...");
                logger.LogInformation($"Finished clearing database. task completed at: {DateTimeOffset.Now} ,total time taken for this operation: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");

                logger.LogInformation("Started copying database...");
                //Copying ParkingLots table
                logger.LogInformation(LogDBStartedTableString("ParkingLot"));
                List<ParkingLot> parkingLots = ParkingLot.GetAll(Program.DBH_FromDB);
                foreach (ParkingLot parkingLot in parkingLots)
                {
                    Program.DBH_ToDB.Create(parkingLot.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("ParkingLot", parkingLots.Count()));
                //Copying Billing table
                logger.LogInformation(LogDBStartedTableString("Billing"));
                List<Billing> billings = Billing.GetAll(Program.DBH_FromDB);
                foreach (Billing billing in billings)
                {
                    Program.DBH_ToDB.Create(billing.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("Billing", billings.Count()));
                //Copying Payment table
                logger.LogInformation(LogDBStartedTableString("Payment"));
                List<Payment> payments = Payment.GetAll(Program.DBH_FromDB);
                foreach (Payment payment in payments)
                {
                    Program.DBH_ToDB.Create(payment.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("Payment", payments.Count()));
                //Copying Reservation table
                logger.LogInformation(LogDBStartedTableString("Reservation"));
                List<Reservation> reservations = Reservation.GetAll(Program.DBH_FromDB);
                foreach (Reservation reservation in reservations)
                {
                    Program.DBH_ToDB.Create(reservation.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("Reservation", reservations.Count()));
                //Copying Session table
                logger.LogInformation(LogDBStartedTableString("Session"));
                List<Session> sessions = Session.GetAll(Program.DBH_FromDB);
                foreach (Session session in sessions)
                {
                    Program.DBH_ToDB.Create(session.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("Session", sessions.Count()));
                //Copying User table
                logger.LogInformation(LogDBStartedTableString("User"));
                List<User> users = User.GetAll(Program.DBH_FromDB);
                foreach (User user in users)
                {
                    Program.DBH_ToDB.Create(user.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("User", users.Count()));
                //Copying UserActivity table
                logger.LogInformation(LogDBStartedTableString("UserActivity"));
                List<UserActivity> userActivities = UserActivity.GetAll(Program.DBH_FromDB);
                foreach (UserActivity userActivity in userActivities)
                {
                    Program.DBH_ToDB.Create(userActivity.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("UserActivity", userActivities.Count()));
                //Copying UserRole table
                logger.LogInformation(LogDBStartedTableString("UserRole"));
                List<UserRole> userRoles = UserRole.GetAll(Program.DBH_FromDB);
                foreach (UserRole userRole in userRoles)
                {
                    Program.DBH_ToDB.Create(userRole.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("UserRole", userRoles.Count()));
                //Copying UserSession table
                logger.LogInformation(LogDBStartedTableString("UserSession"));
                List<UserSession> userSessions = UserSession.GetAll(Program.DBH_FromDB);
                foreach (UserSession userSession in userSessions)
                {
                    Program.DBH_ToDB.Create(userSession.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("UserSession", userSessions.Count()));
                //Copying Vehicle table
                logger.LogInformation(LogDBStartedTableString("Vehicle"));
                List<Vehicle> vehicles = Vehicle.GetAll(Program.DBH_FromDB);
                foreach (Vehicle vehicle in vehicles)
                {
                    Program.DBH_ToDB.Create(vehicle.ToInsertSQL());
                }
                logger.LogInformation(LogDBFinishedTableString("Vehicle", vehicles.Count()));
                logger.LogInformation("Finished copying database...");

                logger.LogInformation($"Finished copying database. task completed at: {DateTimeOffset.Now}, total time taken for this operation: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");
                logger.LogInformation($"Total time taken for entire operation: {(DateTimeOffset.Now - DateTimeOffset.Now.AddMinutes(-1)).TotalSeconds} seconds.");
            }
            await Task.Delay((int) Converter.HourToMilisecond(24), stoppingToken);
        }
    }
}
