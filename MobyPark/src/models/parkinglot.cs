public class ParkingLot : iToTransform
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Adress { get; set; }
    public int Capacity { get; set; }
    public int Reserved { get; set; }
    public decimal Tariff { get; set; }
    public decimal DayTariff { get; set; }
    public DateTime CreatedAt { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    //useful methods to check parking lots state
    public bool HasFreeSpots() => Reserved < Capacity;

    public int GetAvailableSpots() => Capacity - Reserved;
    public bool IsFull() => Reserved >= Capacity;
}