using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
public class Billing : aModel, iDatabase
{
    // I need a public ParkingInfo ofzo voor
    // "parking": {k: v for k, v in parkinglot.items() 
    // if k in ["name", "location", "tariff", "daytariff"]},

    // I need a public sessionInfo ofzo voor
    // "session": {k: v for k, v in session.items() 
    // if k in ["licenseplate", "started", "stopped"]} | {"hours": hours, "days": days},
    public int ID { get; set; }
    public int SessionID { get; set; }
    public int ParkingLotID { get; set; }
    public decimal Amount { get; set; }
    public string tHash { get; set; }
    public decimal Payed { get; set; }
    public decimal Balance { get; set; }

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
                ID = Convert.ToInt32(reader["ID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                tHash = reader["tHash"].ToString(),
                Payed = Convert.ToDecimal(reader["Payed"].ToString()),
                Balance = Convert.ToDecimal(reader["Balance"].ToString()),
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
                ID = Convert.ToInt32(reader["ID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                tHash = reader["tHash"].ToString(),
                Payed = Convert.ToDecimal(reader["Payed"].ToString()),
                Balance = Convert.ToDecimal(reader["Balance"].ToString()),
            };

            returnValue.Add(newBilling);
        }

        conn.Close();
        return returnValue;
    }


    // public static List<Billing> GetAllByUserID(int userID)
    // {
    //     List<Billing> returnValue = [];

    //     MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
    //     MySqlCommand cmd = new MySqlCommand();

    //     conn.Open();
    //     cmd.Connection = conn;
    //     cmd.CommandText = ToSelectByUserIDSQL(userID);

    //     MySqlDataReader reader = cmd.ExecuteReader();
    //     while (reader.Read())
    //     {
    //         Billing newBilling = new()
    //         {
    //             SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
    //             ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
    //             Amount = Convert.ToDecimal(reader["Amount"].ToString()),
    //             tHash = reader["tHash"].ToString(),
    //             Payed = Convert.ToDecimal(reader["Payed"].ToString()),
    //             Balance = Convert.ToDecimal(reader["Balance"].ToString()),
    //         };

    //         returnValue.Add(newBilling);
    //     }

    //     conn.Close();
    //     return returnValue;
    // }

    public override string ToInsertSQL()
    {
        throw new NotImplementedException();
    }

    public override string ToUpdateSQL()
    {
        throw new NotImplementedException();
    }

    public override string ToDeleteSQL()
    {
        throw new NotImplementedException();
    }

    public override string ToSelectSQL()
    {
        throw new NotImplementedException();
    }

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Billing WHERE ID = {id}";
    // public static string ToSelectByUserIDSQL(int id) => "";

    public static string ToSelectAllSQL() => "SELECT * FROM Billing";
}