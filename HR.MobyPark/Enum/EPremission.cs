public enum EPremissions
{
    None,
    Write,
    Read,
    ReadWrite
}

public class Premission
{
    public int premissionInteger {
        get
        {
            return (int) _premission;
        }
        set
        {
            _premission = (EPremissions) value;
        }
    }
    private EPremissions _premission; 

    //Creating a Premission object (no value inputted)
    public Premission() => new Premission(0);

    //Creating a Premission object (value inputted)
    public Premission(int value) => premissionInteger = value;

    public bool HasWritePremission() => 
        _premission == EPremissions.Write|| _premission == EPremissions.ReadWrite ? true : false;

    public bool HasReadPremission() => 
        _premission == EPremissions.Read|| _premission == EPremissions.ReadWrite ? true : false;

    public void SetWrite(bool state)
    {
        if (_premission == EPremissions.ReadWrite)
        {   
            if (!state)
            {
                _premission = EPremissions.Read;   
            }
        }
        else
        {
            if (state)
            {
                if (HasReadPremission())
                {
                    _premission = EPremissions.ReadWrite;
                }
                else
                {
                    _premission = EPremissions.Write;
                }
            } 
            else
            {
                _premission = EPremissions.None;
            }
        }
    }

    public void SetRead(bool state)
    {
        if (_premission == EPremissions.ReadWrite)
        {   
            if (!state)
            {
                _premission = EPremissions.Write;
            }
        }
        else
        {
            if (state)
            {
                if (HasWritePremission())
                {
                    _premission = EPremissions.ReadWrite;
                }
                else
                {
                    _premission = EPremissions.Read;
                }
            }
            else
            {
                _premission = EPremissions.None;
            }
        }
    }
}