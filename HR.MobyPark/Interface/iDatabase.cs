public interface iDatabase
{
    //Standard SQL strings for a object
    public string ToInsertSQL();
    public string ToUpdateSQL();
    public string ToDeleteSQL();
    public string ToSelectSQL();

    //Standard Static SQL strings for the model
    public virtual static string ToSelectAllSQL() {return "IMPLEMENT";}
    public virtual static string ToSelectByIDSQL() {return "IMPLEMENT";}

    //Database interactions for a Object
    public bool CreateDB();
    public bool UpdateDB();
    public bool DeleteDB();
}