namespace CodeKata.BL.DOMAIN;

public class Resource
{
    public string Id { get; set; }
    public string Name { get; set; }
    public TimeOnly StartTime  { get; set; }
    public TimeOnly EndTime  { get; set; }
    public IEnumerable<Skill> Skills { get; set; } =  new List<Skill>();
}