using HR.MobyPark_SessionWorker;

public class Program { 
    public static DBHelper DBHelper = new ();

    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        //Loading database connection variables into a DBConfig object at program launch
        DBConfig? dbConfig = builder.Configuration.GetSection("FromDatabase").Get<DBConfig>();
        //If database connection variables can't be loaded exit program with error
        if (dbConfig is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Check if the HOST and PORT for the DBConfig are filled
        if (dbConfig.HST is null || dbConfig.PRT is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Set the DBConfig into the DBHelper
        DBHelper.dBConfig = dbConfig;

        var host = builder.Build();
        host.Run();
    }
}