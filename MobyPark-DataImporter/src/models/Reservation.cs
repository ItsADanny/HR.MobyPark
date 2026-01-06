public class Reservation : iSQL
{
    // Sample JSON data
    // ========================================================
    // {
    //     "id":"1",
    //     "user_id":"281",
    //     "parking_lot_id":"217",
    //     "vehicle_id":"471",
    //     "start_time":"2025-12-03T11:00:00Z",
    //     "end_time":"2025-12-03T14:00:00Z",
    //     "status":"confirmed",
    //     "created_at":"2025-12-01T11:00:00Z",
    //     "cost":7.5
    // }

    public string id { get; set; }
    public string user_id { get; set; }
    public string parking_lot_id { get; set; }
    public string vehicle_id { get; set; }
    public string start_time { get; set; }
    public string end_time { get; set; }
    public string status { get; set; }
    public string created_at { get; set; }
    public double cost { get; set; }

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