namespace CodeKata.BL;

public interface IFileHandler
{
    public string DeserializeFromStream(Stream stream);
}