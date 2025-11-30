var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

//Loading database connection variables into a DBConfig object at program launch
DBConfig? dbConfig = builder.Configuration.GetSection("Database").Get<DBConfig>();
//If database connection variables can't be loaded exit program with error
if (dbConfig is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
//Check if the HOST and PORT for the DBConfig are filled
if (dbConfig.HST is null || dbConfig.PRT is null) throw new NullReferenceException("Can't load user-secrets into DBConfig");
//Set the DBConfig into the DBHelper
DBHelper.dBConfig = dbConfig;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //This will generate a OpenAPI yaml document 
    //when the application is run in DEV mode
    app.MapOpenApi("/openapi/{documentName}.yaml");
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

//Test endpoint to ping
app.MapPost("ping", () =>
{
    return "Pong";
})
.WithName("Ping");

app.UseHttpsRedirection();
// app.MapSwagger().RequireAuthorization();
app.MapControllers();
app.Run();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

Console.WriteLine($"Server started, Listening to port: {port}");
Log.WriteLine($"Server started, Listening to port: {port}");

//Application records
//==================================================================================================
public record registerDto(string username, string password, string fullname, string email, string phonenumber, DateOnly birthdate);
public record loginDto(string username, string password);
public record createPaymentDto(string transaction, decimal amount);
public record paymentDto(decimal amount, DateOnly date, string method, string issuer, string bank, int sessionID, int parkingLotID);
public record reservationDto(int userID, int parkingLotID, int vehicleID, DateTime startDateTime, DateTime endDateTime);
public record updateReservationDto(DateTime startDateTime, DateTime endDateTime, string status);
public record parkingLotDto(string name, string location, string address, int capacity, decimal tariff, decimal dayTariff, double lat, double lon);