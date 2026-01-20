using HR.MobyPark_UnpaidBillingWorker;

public class Program {

    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        //Loading database connection variables into a DBConfig object at program launch
        DBConfig? dbConfigFromDB = builder.Configuration.GetSection("Database").Get<DBConfig>();
        //If database connection variables can't be loaded exit program with error
        if (dbConfigFromDB is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Check if the HOST and PORT for the DBConfig are filled
        if (dbConfigFromDB.HST is null || dbConfigFromDB.PRT is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
        //Set the DBConfig into the DBHelper
        DBHelper.dBConfig = dbConfigFromDB;

        //Loading SMTP configuration variables into SMTPConfig object at program launch
        SMTPConfig.HST = builder.Configuration.GetValue<string>("SMTP:HST") ?? throw new NullReferenceException("Can't load user-secrets into SMTPConfig");
        SMTPConfig.PRT = builder.Configuration.GetValue<int>("SMTP:PRT");
        SMTPConfig.USR = builder.Configuration.GetValue<string>("SMTP:USR") ?? throw new NullReferenceException("Can't load user-secrets into SMTPConfig");
        SMTPConfig.PWD = builder.Configuration.GetValue<string>("SMTP:PWD") ?? throw new NullReferenceException("Can't load user-secrets into SMTPConfig");

        var host = builder.Build();
        host.Run();
    }
}


