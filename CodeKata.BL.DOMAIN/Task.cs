namespace CodeKata.BL.DOMAIN;

public class Task
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Minutes { get; set; }
    public TimeOnly Deadline { get; set; }
    public Priority Priority { get; set; }
    public Skill  RequiredSkill { get; set; }
}