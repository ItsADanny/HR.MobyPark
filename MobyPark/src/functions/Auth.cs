using System.Web;

public static class Auth
{
    private static Random rand = new Random();

    private static string GenSalt()
    {
        //Generate a cryptographically secure random salt
        byte[] saltBytes = new byte[32];

        //Temporarly initialize a new Cryptographicly secure Random number generator
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        //Return the randomly generated Salt
        return Convert.ToBase64String(saltBytes);
    }

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
}