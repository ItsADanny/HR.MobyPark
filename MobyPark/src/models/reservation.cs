public class Reservation : iToTransform
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ParkingLotId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Cost { get; set; } //Decimal is beter dan double om afronden te voorkomen.
}