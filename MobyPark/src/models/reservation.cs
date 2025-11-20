using Newtonsoft.Json.Linq;

public class Reservation : iToTransform
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ParkingLotId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "active";
    public decimal Amount { get; set; } //Decimal is beter dan double om afronden te voorkomen.

    // public DateTime CreatedAt { get; set; } = DateTime.Now;

    public JObject ToJSON()
    {
        JObject reservation = new JObject
        {
            { "Id", Id },
            { "UserId", UserId },
            { "ParkingLotId", ParkingLotId },
            { "VehicleId", VehicleId },
            { "StartTime", StartTime },
            { "EndTime", EndTime },
            { "Status", Status },
            { "Amount", Amount }
        };
        return reservation;
    }
    public string ToSQLInString()
    {
        return null;
    }
    public string ToSQLUpString()
    {
        return null;
    }

}