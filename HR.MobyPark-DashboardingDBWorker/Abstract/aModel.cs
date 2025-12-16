public abstract class aModel : iDatabase
{
    //IMPLEMENTATION OF THE INTERFACE iDatabase
    //==============================================================================

    //Required to be implemented because these are model specific
    public abstract string ToInsertSQL();

    //virtual because in case a different insert/update/delete is needed
    //the method can be overwritten
    public virtual bool CreateDB(DBHelper dBHelper) => dBHelper.Create(ToInsertSQL()) != 0;
}