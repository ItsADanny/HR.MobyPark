using HR.MobyPark_DashboardingDBWorker;

public class Program {
    public static DBHelper DBH_FromDB = new ();
    public static DBHelper DBH_ToDB = new ();

    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        //Loading database connection variables into a DBConfig object at program launch
        DBConfig? dbConfigFromDB = builder.Configuration.GetSection("FromDatabase").Get<DBConfig>();
        //If database connection variables can't be loaded exit program with error
        if (dbConfigFromDB is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Check if the HOST and PORT for the DBConfig are filled
        if (dbConfigFromDB.HST is null || dbConfigFromDB.PRT is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Set the DBConfig into the DBHelper
        DBH_FromDB.dBConfig = dbConfigFromDB;

        //Loading database connection variables into a DBConfig object at program launch
        DBConfig? dbConfigToDB = builder.Configuration.GetSection("ToDatabase").Get<DBConfig>();
        //If database connection variables can't be loaded exit program with error
        if (dbConfigToDB is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Check if the HOST and PORT for the DBConfig are filled
        if (dbConfigToDB.HST is null || dbConfigToDB.PRT is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Set the DBConfig into the DBHelper
        DBH_ToDB.dBConfig = dbConfigToDB;

        var host = builder.Build();
        host.Run();
    }
}