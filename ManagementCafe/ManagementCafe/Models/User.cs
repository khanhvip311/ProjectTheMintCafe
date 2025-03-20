using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Pass { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
