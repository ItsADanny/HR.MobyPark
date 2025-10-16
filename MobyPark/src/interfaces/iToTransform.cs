using Newtonsoft.Json.Linq;

public interface iToTransform
{
    public JObject ToJSON();
    public string ToSQLInString();
    public string ToSQLUpString();
}