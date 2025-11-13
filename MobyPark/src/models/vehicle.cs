public class Vehicle : iToTransform
{
    //{
    // "id":"1",
    // "user_id":"1",
    // "license_plate":"76-KQQ-7",
    // "make":"Peugeot",
    // "model":"308",
    // "color":"Brown",
    // "year":2024,
    // "created_at":"2024-08-13"}

    //Public properties
    public int ID { get; }
    public int UserID { get; }
    public string LicensePlate { get; }
    public string Make { get; }
    public string Model { get; }
    public string Color {
        get
        {
            return _color;
        } 
    }
    public int ModelYear { get; }
    public DateTime CreateDateTime { get; }
    public DateTime UpdateDateTime { get; }

    //Private variable
    private string _color;

    public Vehicle(int id, int userID, string licensePlate, string make, string model, string color, int modelYear)
    {
        new Vehicle(id, userID, licensePlate, make, model, color, modelYear, DateTime.Now);
    }

    public Vehicle(int id, int userID, string licensePlate, string make, string model, string color, int modelYear, string createDateTime, string? updateDateTime=null)
    {

    }
    
    public Vehicle(int id, int userID, string licensePlate, string make, string model, string color, int modelYear, DateTime createDateTime, DateTime? updateDateTime=null)
    {
        
    }
    
    public void UpdateColor(string newColor)
    {
        _color = newColor;
    }
}