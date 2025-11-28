public class ParkingLot : aDatabase, iDatabase
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Address { get; set; }
    public int Capacity { get; set; }
    public int Reserved { get; set; }
    public decimal Tariff { get; set; }
    public decimal DayTariff { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }

    //useful methods to check parking lots state
    public bool HasFreeSpots() => Reserved < Capacity;
    public int GetAvailableSpots() => Capacity - Reserved;
    public bool IsFull() => Reserved >= Capacity;

    public override string ToInsertSQL() => $"INSERT INTO ParkingLots (Name, Location, Address, Capacity, Tariff, DayTariff, Lat, Lon) VALUES (\"{Name}\", \"{Location}\", \"{Address}\", {Capacity}, {Tariff}, {DayTariff}, {Latitude}, {Longitude})";

    public override string ToUpdateSQL() => $"UPDATE ParkingLots SET Name = \"{Name}\", Location = \"{Location}\", Address = \"{Address}\", Capacity = {Capacity}, Tariff = {Tariff}, DayTariff = {DayTariff}, Lat = {Latitude}, Lon = {Longitude} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM ParkingLots WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM ParkingLots WHERE ID = {ID}";
}