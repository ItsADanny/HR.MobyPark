namespace HR.MobyPark_DashboardingDBWorker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                DateTime startMoment = DateTime.Now;
                logger.LogInformation("HR.MobyPark Dashboarding Database Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("Started database cleaning");
                Program.DBH_ToDB.Delete(LegacyUser.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(ParkingLot.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Reservation.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Session.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Payment.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(User.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserActivity.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserRole.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(UserSession.ToDeleteAllSQL());
                Program.DBH_ToDB.Delete(Vehicle.ToDeleteAllSQL());
                logger.LogInformation("Finished cleaning, starting duplication");
                logger.LogInformation("Retrieving LegacyAccount records from Production database");
                //Retriev values from original database, and let the DBHelper create 
                //the records in the dashboard database
                List<LegacyUser>? legacyUsers = LegacyUser.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting LegacyAccount records into Dashboard database");
                Program.DBH_ToDB.Create(legacyUsers);
                //Set to null to remove everything in this variable from memory
                //We DONT want a bloated memory hungry service-worker...
                //(We are not going to make the next Google Chrome)
                legacyUsers = null;
                logger.LogInformation("Retrieving ParkingLot records from Production database");
                List<ParkingLot>? ParkingLots = ParkingLot.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting ParkingLot records into Dashboard database");
                Program.DBH_ToDB.Create(ParkingLots);
                ParkingLots = null;

                logger.LogInformation("Retrieving Reservation records from Production database");
                List<Reservation>? Reservations = Reservation.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting Reservation records into Dashboard database");
                Program.DBH_ToDB.Create(Reservations);
                Reservations = null;

                logger.LogInformation("Retrieving Session records from Production database");
                List<Session>? Sessions = Session.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting Session records into Dashboard database");
                Program.DBH_ToDB.Create(Sessions);
                Sessions = null;

                logger.LogInformation("Retrieving Payment records from Production database");
                List<Payment>? Payments = Payment.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting Payment records into Dashboard database");
                Program.DBH_ToDB.Create(Payments);
                Payments = null;

                logger.LogInformation("Retrieving UserActivity records from Production database");
                List<UserActivity>? UserActivities = UserActivity.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting UserActivity records into Dashboard database");
                Program.DBH_ToDB.Create(UserActivities);
                UserActivities = null;

                logger.LogInformation("Retrieving User records from Production database");
                List<User>? Users = User.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting User records into Dashboard database");
                Program.DBH_ToDB.Create(Users);
                Users = null;

                logger.LogInformation("Retrieving UserRole records from Production database");
                List<UserRole>? UserRoles = UserRole.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting UserRole records into Dashboard database");
                Program.DBH_ToDB.Create(UserRoles);
                UserRoles = null;

                logger.LogInformation("Retrieving Vehicle records from Production database");
                List<Vehicle>? Vehicles = Vehicle.GetAll(Program.DBH_FromDB);
                logger.LogInformation("Inserting Vehicle records into Dashboard database");
                Program.DBH_ToDB.Create(Vehicles);
                Vehicles = null;

                TimeSpan completeTime = DateTime.Now - startMoment;
                logger.LogInformation($"Operation finished at {DateTimeOffset.Now}, Time to Complete: {completeTime:HH:MM}");
            }
            await Task.Delay(Converter.DayToMilisecond(1), stoppingToken);
        }
    }
}
