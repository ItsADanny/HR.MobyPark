public class UserSession
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public string Token { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
}