using Newtonsoft.Json.Linq;

public class Userrole : iToTransform
{
    public int ID { get; }
    public string Name { 
        get { return _name; } 
    }
    public bool ReadPermission
    {
        get { return General.IntToBool(_readPermission); }
    }

    public bool WritePermission
    {
        get { return General.IntToBool(_writePermission); }
    }
    public bool AdminReadPermission
    {
        get { return General.IntToBool(_adminReadPermission); }
    }

    public bool AdminWritePermission
    {
        get { return General.IntToBool(_adminWritePermission); }
    }

    private string _name;
    private int _readPermission;
    private int _writePermission;
    private int _adminReadPermission;
    private int _adminWritePermission;

    public Userrole(int id, string name, int read, int write, int adminRead, int adminWrite)
    {
        ID = id;
        _name = name;
        _readPermission = read;
        _writePermission = write;
        _adminReadPermission = adminRead;
        _adminWritePermission = adminWrite;
    }

    public void UpdateName(string to)
    {
        Log.UnsavedChange("Userrole", ID, "Name", _name, to);
        _name = to;
    }

    public void UpdateRead(bool to)
    {
        Log.UnsavedChange("Userrole", ID, "READ Permission", ReadPermission, to);
        _writePermission = General.BoolToInt(to);
    }

    public void UpdateWrite(bool to)
    {
        Log.UnsavedChange("Userrole", ID, "WRITE Permission", WritePermission, to);
        _writePermission = General.BoolToInt(to);
    }

    public void UpdateAdminRead(bool to)
    {
        Log.UnsavedChange("Userrole", ID, "Admin READ Permission", AdminReadPermission, to);
        _writePermission = General.BoolToInt(to);
    }
    
    public void UpdateAdminWrite(bool to)
    {
        Log.UnsavedChange("Userrole", ID, "Admin WRITE Permission", AdminWritePermission, to);
        _writePermission = General.BoolToInt(to);
    }

    public string ToSQLInString() => $"{ID}, {Name}, {_readPermission}, {_writePermission}, {_adminReadPermission}, {_adminWritePermission}";
    public string ToSQLUpString() => $"{Name}, {_readPermission}, {_writePermission}, {_adminReadPermission}, {_adminWritePermission} WHERE ID = {ID}";
    public JObject ToJSON()
    {
        return new JObject(
            new JProperty("ID", ID),
            new JProperty("Name", Name),
            new JProperty("Permission" , new JObject(
                new JProperty("Read", ReadPermission),
                new JProperty("Write", WritePermission),
                new JProperty("AdminRead", AdminReadPermission),
                new JProperty("AdminWrite", AdminWritePermission)
            ))
        );
    }
}