public class Billing : aModel, iDatabase
{
    // I need a public ParkingInfo ofzo voor
    // "parking": {k: v for k, v in parkinglot.items() 
    // if k in ["name", "location", "tariff", "daytariff"]},

    // I need a public sessionInfo ofzo voor
    // "session": {k: v for k, v in session.items() 
    // if k in ["licenseplate", "started", "stopped"]} | {"hours": hours, "days": days},
    public decimal Amount { get; set; }

    // public string TransactionHash ofzo
    public decimal Payed { get; set; }
    public decimal Balance { get; set; }

    public static JObject ToJSON(Billing billing)
    {
        JObject returnValue = new()
        {
            new JProperty("amount", billing.Amount),
            new JProperty("payed", billing.Payed),
            new JProperty("balance", billing.Balance),
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

    // deze nog afmaken
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
            Billing returnValue = new()
            {
                /// HIER NOG
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    // deze ook
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
            Billing newBilling = new()
            {
                //  hier nog
            };

            returnValue.Add(newBilling);
        }

        conn.Close();
    }

    // deze ook
    public static List<Billing> GetAllByUserID(int userID)
    {
        List<Billing> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByUserIDSQL(userID);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Billing newBilling = new()
            {
                // hier weer
            };

            returnValue.Add(newBilling);
        }

        conn.Close();
        return returnValue;
    }

}