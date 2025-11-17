using Newtonsoft.Json.Linq;

public class Payment: iToTransform
{
    public int ID { get; set; }
    public string TransactionID { get; set; }
    public int ParkingLotID { get; set; }
    public int SessionID { get; set; }
    public int UserID { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Method { get; set; }
    public string Issuer { get; set; }
    public string Bank { get; set; }
    public string Hash { get; set; }
    public DateTime CompletedDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }

    public JObject ToJSON()
    {
        throw new NotImplementedException();
    }

    public string ToSQLInString()
    {
        throw new NotImplementedException();
    }

    public string ToSQLUpString()
    {
        throw new NotImplementedException();
    }
}