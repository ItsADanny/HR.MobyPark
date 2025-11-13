public class CPayment
{
    public static IResult GetMyPayments(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        var session = Auth.GetSession(token);
        if (session == null)
        {
            return Results.NotFound("User not found");
        }
        string username = CDatabase.GetUsernameById(session.UserID);
        if (username == null)
        {
            return Results.NotFound("User not found");
        }
        var payments = CDatabase.GetPaymentsBySession(username);
        return Results.Ok(payments);
    }

    public static IResult CreatePayment(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        var session = Auth.GetSession(token);
        if (session == null)
        {
            return Results.NotFound("User not found");
        }
        string username = CDatabase.GetUsernameById(session.UserID);
        if (username == null)
        {
            return Results.NotFound("User not found");
        }
    return Results.Ok();
    }
}