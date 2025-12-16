using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public static class DatabaseHelper
{
    private static string HST = "145.24.237.97";
    private static string PRT = "8089";
    private static string USR = "MobyParkAPI";
    private static string PSW = "g7qZnf09Ugj2";
    private static string DBL = "MobyPark_DB";
    private static string connectionString = $"server={HST};port = {PRT};uid={USR};pwd={PSW};database={DBL}";

    public static int INSERT (string insertSQLString)
    {
        try {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = insertSQLString;
            cmd.ExecuteNonQuery();
            int insertID = Convert.ToInt32(cmd.LastInsertedId);

            conn.Close();

            return insertID;
        }
        catch (Exception e)
        {
            Console.WriteLine(connectionString);
            Console.WriteLine(insertSQLString);
            Console.WriteLine(e);
            Thread.Sleep(20000);
            return 0;
        }
    }

    public static bool Update(string updateSQLString) => INSERT(updateSQLString) != 0 ? true : false;

    public static List<User> GetUsers()
    {
        try {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM LegacyAccounts";
            
            using MySqlDataReader reader = cmd.ExecuteReader();
            List<User> returnValue = [];
            
            while (reader.Read())
            {
                int? newUserID = null;
                if (reader["NewUserID"] is not DBNull) newUserID = Convert.ToInt32(reader["NewUserID"].ToString());

                User user = new ()
                {
                    id = reader["ID"].ToString(),
                    username = reader["Username"].ToString(),
                    password = reader["Password"].ToString(),
                    name = reader["Name"].ToString(),
                    email = reader["Email"].ToString(),
                    phone = reader["Phone"].ToString(),
                    role = reader["Role"].ToString(),
                    created_at = reader["CreatedAt"].ToString(),
                    birth_year = Convert.ToInt32(reader["BirthYear"].ToString()),
                    active = Convert.ToBoolean(reader["Active"].ToString()),
                    NewUserID = newUserID
                };

                returnValue.Add(user);
            }

            return returnValue;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(20000);
            return [];
        }
    } 

    public static NewUser? GetUserByEmail(string email)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            string query = $"SELECT * FROM Users WHERE Email = \"{email}\"";
            cmd.Connection = conn;
            cmd.CommandText = query;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                NewUser user = new ()
                {
                    ID = Convert.ToInt32(reader["ID"].ToString()),
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    BirthDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["BirthDate"].ToString()))
                };
                conn.Close();
                return user;
            }
            else
            {
                conn.Close();
                return null;
            } 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(100000);
            return null;
        }
    }

    public static int? GetNewUserIDFromOld(string orgID)
    {
        int? returnValue = null;

        try {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = $"SELECT * FROM LegacyAccounts WHERE OrigID = {orgID}";
            
            using MySqlDataReader reader = cmd.ExecuteReader();
            
            if(reader.Read())
            {
                returnValue = Convert.ToInt32(reader["NewUserID"].ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(3000);
        }

        return returnValue;
    }

    public static bool DoesUserExist(string email) => GetUserByEmail(email) == null ? false : true;
}