using Microsoft.AspNetCore.Identity;

namespace bezkie.core.Entities;

public class User : IdentityUser<long>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<RSVP> RSVPs { get; set; }
}
