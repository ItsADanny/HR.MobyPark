public class User : iToTransform
{
    //Original fields on which this class is based

    //{"id": "1", 
    // "username": "cindy.leenders42", 
    // "password": "6b37d1ec969838d29cb611deaff50a6b", 
    // "name": "Cindy Leenders", 
    // "email": "cindyleenders@upcmail.nl", 
    // "phone": "+310792215694", 
    // "role": "USER", 
    // "created_at": "2017-10-06", 
    // "birth_year": 1937, 
    // "active": true

    public int ID { get; }
    public string Username { get; set; }
    private string _password;
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public int BirthYear { get; set; }
    public bool Active { get; set; }
    public DateTime CreateDateTime { get; }
    public DateTime UpdateDateTime { get; }

    //This method will simply return a true or false when checking the users password,
    //this will make sure that the password is never exposed or to be accessed outside of this class
    public bool IsPasswordCorrect(string guess, string salt) => Auth.Hash(guess, salt)[0] == _password ? true : false;
    
    public bool IsPasswordCorrect(string guess) => Auth.Hash(guess, CDatabase.GetSalt(ID))[0] == _password ? true : false;
}