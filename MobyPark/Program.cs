var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/register", (HttpRequest request) =>
{
    // hierin komt de functie
    CUser.Register(request);
})
.WithName("PostRegister");

app.MapPost("/login", () =>
{

})

.WithName("PostLogin");

app.MapGet("/logout", () =>
{

})
.WithName("GetLogout");

app.MapGet("/payments", (HttpRequest request) =>
{
    CPayment.GetMyPayments(request);
}).WithName("GetPayments");

app.MapPost("/payments", (HttpRequest request) =>
{
    CPayment.CreatePayment(request);
}).WithName("PostPayments");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Log.WriteLine($"Server started, Listening to port: {port}");
app.Run();