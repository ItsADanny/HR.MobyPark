using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
public class Billing : aModel, iDatabase
{
    public int ID { get; set; }
    public int SessionID { get; set; }
    public int ParkingLotID { get; set; }
    public decimal Amount { get; set; }
    public string tHash { get; set; }
    public decimal Payed { get; set; }
    public decimal Balance { get; set; }
    public DateTime CompletedDateTime { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    public static JObject ToJSON(Billing billing)
    {
        JObject returnValue = new()
        {
            new JProperty("id", billing.ID),
            new JProperty("sessionID", billing.SessionID),
            new JProperty("parkingLotID", billing.ParkingLotID),
            new JProperty("amount", billing.Amount),
            new JProperty("tHash", billing.tHash),
            new JProperty("payed", billing.Payed),
            new JProperty("balance", billing.Balance),
            new JProperty("completedDateTime", billing.CompletedDateTime),
            new JProperty("createDateTime", billing.CreateDateTime),
            new JProperty("updateDateTime", billing.UpdateDateTime)

        };

        return returnValue;
    }

    public static JArray ToJSON(List<Billing> billings)
    {
        JArray returnValue = new();

        foreach (Billing billing in billings)
        {
            returnValue.Add(ToJSON(billing));
        }

        return returnValue;
    }

    public static Billing? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Billing returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                tHash = reader["tHash"].ToString(),
                Payed = Convert.ToDecimal(reader["Payed"].ToString()),
                Balance = Convert.ToDecimal(reader["Balance"].ToString()),
                CompletedDateTime = Convert.ToDateTime(reader["CompletedDateTime"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Billing> GetAll()
    {
        List<Billing> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Billing newBilling = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                tHash = reader["tHash"].ToString(),
                Payed = Convert.ToDecimal(reader["Payed"].ToString()),
                Balance = Convert.ToDecimal(reader["Balance"].ToString()),
                CompletedDateTime = Convert.ToDateTime(reader["CompletedDateTime"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newBilling);
        }

        conn.Close();
        return returnValue;
    }


    public override string ToInsertSQL() => $"INSERT INTO Billing (SessionID, ParkingLotID, Amount, tHash, Payed, Balance, CompletedDateTime) VALUES ({SessionID}, {ParkingLotID}, {Amount}, \"{tHash}\", {Payed}, {Balance}, {CompletedDateTime})";

    public override string ToUpdateSQL() => $"UPDATE Billing SET SessionID = {SessionID}, ParkingLotID = {ParkingLotID}, Amount = {Amount}, tHash = \"{tHash}\", Payed = {Payed}, Balance = {Balance}, CompletedDateTime = {CompletedDateTime:YYYY-MM-DD hh:mm:ss} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Billing WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Billing WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Billing WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Billing";

    // ====================================================================================
    // Billing Data Generation (optimized for N+1 query prevention)
    // ====================================================================================
    
    /// <summary>
    /// Gets billing data for a specific user as List<Billing>. Optimized to avoid N+1 queries.
    /// Follows the same pattern as Payment.GetAllByUserID and ParkingLot.GetAll.
    /// </summary>
    public static List<Billing> GetBillingDataForUser(int userId)
    {
        List<Billing> billingData = new();

        // Get all sessions for this user (single query)
        List<Session> userSessions = Session.GetAllByUserID(userId);
        if (userSessions.Count == 0) return billingData;

        // Get all payments for this user once (avoids N+1 query problem)
        List<Payment> userPayments = Payment.GetAllByUserID(userId);
        
        // Group payments by SessionID for efficient lookup
        Dictionary<int, decimal> paymentsBySession = GroupPaymentsBySession(userPayments);

        // Get all parking lots needed (cache for efficient lookup)
        HashSet<int> parkingLotIds = userSessions.Select(s => s.ParkingLotId).ToHashSet();
        Dictionary<int, ParkingLot> parkingLots = GetParkingLotsDictionary(parkingLotIds);

        // Build billing records for each session
        foreach (Session session in userSessions)
        {
            if (!parkingLots.TryGetValue(session.ParkingLotId, out ParkingLot? parkingLot))
                continue;

            decimal payed = paymentsBySession.GetValueOrDefault(session.ID, 0);
            decimal amount = (decimal)session.Amount;
            string transactionHash = $"{session.ID}-{session.UserID}-{parkingLot.ID}";

            Billing billing = new()
            {
                SessionID = session.ID,
                ParkingLotID = parkingLot.ID,
                Amount = amount,
                tHash = transactionHash,
                Payed = payed,
                Balance = amount - payed,
                CompletedDateTime = session.EndDateTime,
                CreateDateTime = DateTime.Now
            };
            
            billingData.Add(billing);
        }

        return billingData;
    }

    /// <summary>
    /// Groups payments by SessionID and calculates total paid amount per session.
    /// </summary>
    private static Dictionary<int, decimal> GroupPaymentsBySession(List<Payment> payments)
    {
        Dictionary<int, decimal> result = new();
        foreach (Payment payment in payments)
        {
            if (result.ContainsKey(payment.SessionID))
                result[payment.SessionID] += payment.Amount;
            else
                result[payment.SessionID] = payment.Amount;
        }
        return result;
    }

    /// <summary>
    /// Gets parking lots by their IDs and returns as a dictionary for efficient lookup.
    /// </summary>
    private static Dictionary<int, ParkingLot> GetParkingLotsDictionary(HashSet<int> parkingLotIds)
    {
        Dictionary<int, ParkingLot> result = new();
        List<ParkingLot> allParkingLots = ParkingLot.GetAll();
        
        foreach (ParkingLot lot in allParkingLots)
        {
            if (parkingLotIds.Contains(lot.ID))
                result[lot.ID] = lot;
        }
        
        return result;
    }

}