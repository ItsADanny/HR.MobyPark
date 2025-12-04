public class ParkingLot
{
    // Sample JSON data
    // ========================================================
    // {
    //     "transaction":"82724346a21e6ddf8da490f7c49355bcBB",
    //     "amount":16.5,
    //     "initiator":"natasjadewit",
    //     "created_at":"26-03-2020 05:40:151585197647",
    //     "completed":"26-03-2020 05:40:151585197655",
    //     "hash":"3524ea52-c49a-4a5b-9395-82f2a152368f",
    //     "t_data":
    //         {
    //             "amount":16.5,
    //             "date":"2020-03-26 05:40:47",
    //             "method":"bancontact",
    //             "issuer":"9I2G36YZ",
    //             "bank":"Bunq"
    //         },
    //     "session_id":"1",
    //     "parking_lot_id":"1"
    // }

    public string transaction { get; set; }
    public double amount { get; set; }
    public string initiator { get; set; }
    public string created_at { get; set; }
    public string completed { get; set; }
    public string hash { get; set; }
    public BankIssue t_data { get; set; }
    public string session_id { get; set; }
    public string parking_lot_id { get; set; }
}