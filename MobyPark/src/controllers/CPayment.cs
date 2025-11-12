public class CPayment
{
    public static IResult GetMyPayments(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized("Unauthorized: Invalid or missing session token");
        }

        var session = Auth.GetSession(token);
        if (session == null)
        {
            return Results.Json(
                "Unauthorized: Invalid or missing session token",
                statusCode: 401
            );
        }
        string username = CDatabase.GetUsernameById(session.UserID);
        if (username == null)
        {
            return Results.Json(
                "User not found",
                statusCode: 404
            );
        }
        var payments = CDatabase.GetPaymentsBySession(username);
        return Results.Ok(payments);
    }

}