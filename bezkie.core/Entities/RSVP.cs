using bezkie.core.Enums;

namespace bezkie.core.Entities;

public class RSVP
{
    public long Id { get; set; }
    public long BookId { get; set; }
    public long CustomerId { get; set; }
    public DateTime StatusAt { get; set; } = DateTime.UtcNow;
    public RSVPStatus Status { get; set; }
    public User Customer { get; set; }
    public Book Book { get; set; }
}
