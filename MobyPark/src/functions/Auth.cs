using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public static class Auth
{
    private static Random rand = new Random();

    private static string GenSalt(int size = 32)
    {
        //Generate a cryptographically secure random salt
        byte[] saltBytes = new byte[size];

        //Temporarly initialize a new Cryptographicly secure Random number generator
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        //Return the randomly generated Salt
        return Convert.ToBase64String(saltBytes);
    }

    public static string GenAuthToken() => GenSalt(128);

    public static string[] Hash(string password) => Hash(password, GenSalt());

    public static string[] Hash(string password, string salt)
    {
        //Turn the Password and Salt into their Bytes value
        var pBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var sBytes = System.Text.Encoding.UTF8.GetBytes(salt);

        //Values:
        //-------------------------------
        //Iterations : 4096
        //Hash       : SHA-512
        //Length     : 32

        //Hash the password using the salt, iteration, Hash algorithm and the set length
        var keyder = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2(pBytes, sBytes, 4096, new System.Security.Cryptography.HashAlgorithmName("SHA512"), 32);

        //Return the hashed password string
        return [Convert.ToHexString(keyder), salt];
    }

    public static string LegacyHash(string password)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(password); // Convert string to bytes using UTF-8
        byte[] hashBytes;

        using (MD5 md5 = MD5.Create())
        {
            hashBytes = md5.ComputeHash(dataBytes); // Compute the MD5 hash
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2")); // "x2" ensures two lowercase hex digits
        }

        return sb.ToString();
    }

    public static bool IsTokenFilled(HttpRequest request)
    {
        string? token = request.Headers["Token"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            return true;
        }
        return false;
    }

    public static Session? GetSession(Database DBConfig, string authToken)
    {
        MySqlConnection conn = new MySqlConnection(DBConfig.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Sessions WHERE Token = {authToken}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Session returnValue = new(Convert.ToInt32(reader["ID"].ToString), Convert.ToInt32(reader["UserID"].ToString), reader["Token"].ToString(), );
            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }
}