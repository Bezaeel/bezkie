namespace bezkie.core.Entities;

public class Book : BaseEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<RSVP> RSVPs { get; set; }
}
