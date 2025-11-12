using Newtonsoft.Json.Linq;

public class Payments : iToTransform
{
    public string Transaction { get; set; }
    public decimal Amount { get; set; }
    public string Initiator { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Completed { get; set; }
    public string Hash { get; set; }
    public string SessionId { get; set; }
    public string ParkingLotId { get; set; }

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