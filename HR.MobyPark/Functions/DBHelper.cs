using MySql.Data.MySqlClient;

public static class DBHelper
{
    public static DBConfig dBConfig {private get; set;}

    public static int? Create(string sqlstatement)
    {
        try {
            MySqlConnection conn = new MySqlConnection(dBConfig.GetConnectionSTR());
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sqlstatement;
            cmd.ExecuteNonQuery();

            //This needs to be repaired, this will currently return a 0 (which is not really what happend)
            // cmd.CommandText = "SELECT last_insert_id() AS id";
            // MySqlDataReader reader = cmd.ExecuteReader();
            // reader.Read();
            // int insertID = Convert.ToInt32(reader["id"].ToString());

            conn.Close();

            return 1;
        }
        catch (Exception e)
        {
            //DEBUGGING
            Console.WriteLine(dBConfig.GetConnectionSTR());
            Console.WriteLine(sqlstatement);
            Console.WriteLine(e);
            Thread.Sleep(20000);
            return 0;
        }
    }

    public static bool Update(string sqlstatement) => Create(sqlstatement) != 0 ? true : false;
    public static bool Delete(string sqlstatement) => Update(sqlstatement);

    public static string GetConnectionSTR() => dBConfig.GetConnectionSTR();
}