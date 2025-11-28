public interface iDatabase
{
    //Standard SQL strings for a object
    public string ToInsertSQL();
    public string ToUpdateSQL();
    public string ToDeleteSQL();
    public string ToSelectSQL();

    //Database interactions for a Object
    public bool CreateDB();
    public bool UpdateDB();
    public bool DeleteDB();
}