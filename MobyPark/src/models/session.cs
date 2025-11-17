public class Session
{
    public int ID { get; set; }
    public int ParkingLotId { get; set; }
    public string LicensePlate { get; set; }
    public int VehicleID { get; set; }
    public int UserID { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Duration { get; set; }
    public double Amount { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
}