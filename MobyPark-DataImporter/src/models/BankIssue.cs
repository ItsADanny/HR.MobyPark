public class BankIssue
{
    // Sample JSON data
    // ========================================================
    // {
    //     "amount":16.5,
    //     "date":"2020-03-26 05:40:47",
    //     "method":"bancontact",
    //     "issuer":"9I2G36YZ",
    //     "bank":"Bunq"
    // }

    public double amount { get; set; }
    public string date { get; set; }
    public string method { get; set; }
    public string issuer { get; set; }
    public string bank { get; set; }
}