using MySql.Data.MySqlClient;

public class DBHelper
{
    public DBConfig dBConfig {private get; set;}

    public int? Create(string sqlstatement)
    {
        if (dBConfig is null) return null;
        try {
            MySqlConnection conn = new MySqlConnection(dBConfig.GetConnectionSTR());
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sqlstatement;
            cmd.ExecuteNonQuery();
            // int insertID = Convert.ToInt32(cmd.LastInsertedId);

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

    public int? Create<T>(List<T> data) where T : aModel, iDatabase
    {
        if (dBConfig is null) return null;
        int succes = 0;

        foreach (T value in data) {
            try {
                MySqlConnection conn = new MySqlConnection(dBConfig.GetConnectionSTR());
                MySqlCommand cmd = new MySqlCommand();

                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = value.ToInsertSQL();
                cmd.ExecuteNonQuery();
                // int insertID = Convert.ToInt32(cmd.LastInsertedId);

                conn.Close();

                succes++;
            }
            catch (Exception e)
            {
                //DEBUGGING
                Console.WriteLine(dBConfig.GetConnectionSTR());
                Console.WriteLine(value.ToInsertSQL());
                Console.WriteLine(e);
                Thread.Sleep(20000);
            }
        }

        if (succes == data.Count()) return 1;
        return 0;
    }

    public bool Update(string sqlstatement) {
        int? result = Create(sqlstatement);
        return result != 0 || result is not null;
    }

    public bool Delete(string sqlstatement) => Update(sqlstatement);

    public string GetConnectionSTR() {
        if (dBConfig is null) return "DBHelper not initialized correctly, please initialize the DBHelper by providing it a DBConfig";
        return dBConfig.GetConnectionSTR();
    }
}