public class Database
{
    public string HST { get; set; }
    public string PRT { get; set; }
    public string USR { get; set; }
    public string PSW { get; set; }
    public string DBL { get; set; }

    public string GetConnectionSTR()
    {
        if (HST is null && USR is null && PSW is null && DBL is null)
        {
            throw new NullReferenceException("All database secrets must be filled!");
        }
        return $"server={HST};port={PRT};uid={USR};pwd={PSW};database={DBL}";
    }
}