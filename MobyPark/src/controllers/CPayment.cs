
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

    public static IResult CreatePayment(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        string transaction = request.Query["Transaction"];
        int amount = int.Parse(request.Query["Amount"]);

        if (string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        UserSession session = Auth.GetUserSession(null, token);
        if (session == null)
        {
            return Results.NotFound("Session not found");
        }

        if (string.IsNullOrEmpty(transaction) || amount <= 0)
        {
            return Results.BadRequest("Require field missing");
        }

        int parkingLotID = 0; // to be implemented
        int sessionID = 0; // to be implemented
        
        // add payment creation logic here
        string transactiondone = CDatabase.AddPaymentTransaction(
            transaction, 
            parkingLotID,
            sessionID,
            session.UserID, 
            amount, 
            "Unknown", // method is not provided in the request, assuming a default value of "Unknown"
            "Unknown", // issuer is not provided in the request, assuming a default value of "
            "Unknown",  // bank is not provided in the request, assuming a default value of "Unknown"
            session.CreateDateTime, 
            session.CreateDateTime);

        return Results.Ok();
    }

    public static IResult Refund(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        int amount = int.Parse(request.Query["Amount"]);

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
        if (amount <= 0)
        {
            return Results.BadRequest("Require field missing");
        }
        
        int parkingLotID = 0; // to be implemented
        int sessionID = 0; // to be implemented
        string transaction = $"Refund {session.UserID}"; // to be implemented

        // add payment creation logic here
        string transactiondone = CDatabase.AddPaymentTransaction(
            transaction, 
            parkingLotID,
            sessionID,
            session.UserID, 
            amount, 
            "Unknown", // method is not provided in the request, assuming a default value of "Unknown"
            "Unknown", // issuer is not provided in the request, assuming a default value of "
            "Unknown",  // bank is not provided in the request, assuming a default value of "Unknown"
            session.CreateDateTime, 
            session.CreateDateTime);

        return Results.Ok();
    }
}