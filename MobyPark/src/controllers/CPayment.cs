
public static class CPayment
{
    public static IResult GetMyPayments(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        UserSession session = Auth.GetUserSession(null, token);
        if (session == null)
        {
            return Results.NotFound("Session not found");
        }

        Payment[] payments = CDatabase.GetAllPaymentsBySession(session.UserID);
        foreach (var payment in payments)
        {
            payment.SessionID = 0;
            payment.Hash = null; 
        }
        return Results.Ok(payments);
    }

    public static IResult GetPaymentById(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        int ID = int.Parse((string)request.RouteValues["id"]);

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        UserSession session = Auth.GetUserSession(null, token);
        if (session == null)
        {
            return Results.NotFound("Session not found");
        }

        if (!CDatabase.IsUserAdmin(session.UserID))
        {
            return Results.Unauthorized();
        }

        var payment = CDatabase.GetPaymentById(ID);
        if (payment == null)
        {
            return Results.NotFound("Payment not found");
        }

        payment.SessionID = 0;
        payment.Hash = null;
        return Results.Ok(payment);
    }

    // public static IResult CreatePayment(HttpRequest request)
    // {
    //     string token = request.Headers["Authorization"].FirstOrDefault();
    //     string transaction = request.Query["transaction"];
    //     int amount = int.Parse(request.Query["amount"]);

    //     if (string.IsNullOrEmpty(token)|| string.IsNullOrEmpty(transaction) || amount <= 0)
    //     {
    //         return Results.Unauthorized();
    //     }

    //     UserSession session = Auth.GetUserSession(null, token);
    //     if (session == null)
    //     {
    //         return Results.NotFound("Session not found");
    //     }
        
    //     // add payment creation logic here
    //     string transactiondone = AddPaymentTransaction(session.UserID, transaction, amount);

    //     return Results.Ok();
    // }
}