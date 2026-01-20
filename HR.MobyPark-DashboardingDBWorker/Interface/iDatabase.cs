public interface iDatabase
{
    //Standard SQL strings for a object
    public string ToInsertSQL();

    //Standard Static SQL strings for the model
    public virtual static string ToSelectAllSQL() {return "IMPLEMENT";}
    public virtual static string ToDeleteAllSQL() {return "IMPLEMENT";}

    //Database interactions for a Object
    public bool CreateDB(DBHelper dBHelper) => dBHelper.Create(ToInsertSQL()) != 0;
}