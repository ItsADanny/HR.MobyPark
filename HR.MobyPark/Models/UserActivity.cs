public class UserActivity : aModel, iDatabase
{
    public int ID {get; set;}
    public int UserID {get; set;}
    public string Token {get; set;}
    public string Endpoint {get; set;}
    public EHTTPMethod Method {get; set;}

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO UserActivity (UserID, Token, Endpoint, Method, CreateDateTime) VALUES ({UserID}, \"{Token}\", \"{Endpoint}\", \"{Method}\", NOW())";

    public override string ToUpdateSQL() => $"UPDATE UserActivity SET UserID = {UserID}, Token = \"{Token}\", Endpoint = \"{Endpoint}\", Method = \"{Method}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserActivity WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserActivity WHERE ID = {ID}";

    public static void SaveActivity(UserSession userSession, string endpoint, EHTTPMethod eHTTPMethod)
    {
        UserActivity newUserActivity = new()
        {
            UserID = userSession.UserID,
            Token = userSession.Token,
            Endpoint = endpoint,
            Method = eHTTPMethod
        };

        if (newUserActivity.CreateDB())
        {
            Log.WriteLine("UserActivity:SaveActivity():CreateDB(), Insertion successful, UserActivity saved to Database");
        } 
        else
        {
            Log.WriteErrorLine("UserActivity:SaveActivity():CreateDB(), Insertion failed, please check why the insertion failed");
        }
    }

    public static void SaveActivity(string token, string endpoint, EHTTPMethod eHTTPMethod)
    {
        UserSession? foundUserSession = UserSession.GetUserSession(token);
        if (foundUserSession is not null) SaveActivity(foundUserSession, endpoint, eHTTPMethod);
    }
}