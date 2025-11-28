public abstract class aDatabase : iDatabase
{
    //Required to be implemented because these are model specific
    public abstract string ToInsertSQL();

    public abstract string ToUpdateSQL();

    public abstract string ToDeleteSQL();

    public abstract string ToSelectSQL();

    //virtual because in case a different insert/update/delete is needed
    //the method can be overwritten
    public virtual bool CreateDB() => DBHelper.Create(ToInsertSQL()) != 0;

    public virtual bool UpdateDB() => DBHelper.Update(ToUpdateSQL());

    public virtual bool DeleteDB() => DBHelper.Delete(ToDeleteSQL());
}