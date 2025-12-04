public class Vehicle : iSQL
{
    // Sample JSON data
    // ========================================================
    // {
    //     "id":"1",
    //     "user_id":"1",
    //     "license_plate":"76-KQQ-7",
    //     "make":"Peugeot",
    //     "model":"308",
    //     "color":"Brown",
    //     "year":2024,
    //     "created_at":"2024-08-13"
    // }

    public string id { get; set; }
    public string user_id { get; set; }
    public string license_plate { get; set; }
    public string make { get; set; }
    public string model { get; set; }
    public string color { get; set; }
    public int year { get; set; }
    public string created_at { get; set; }

    public string ToDeleteSQL()
    {
        throw new NotImplementedException();
    }

    public string ToInsertSQL()
    {
        throw new NotImplementedException();
    }

    public string ToUpdateSQL()
    {
        throw new NotImplementedException();
    }
}