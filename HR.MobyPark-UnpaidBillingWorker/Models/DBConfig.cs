public class DBConfig
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
            throw new NullReferenceException("All required database secrets must be filled!");
        }
        if (PRT == "" || PRT is null) return $"server={HST};uid={USR};pwd={PSW};database={DBL}";
        return $"server={HST};port={PRT};uid={USR};pwd={PSW};database={DBL}";
    }
}