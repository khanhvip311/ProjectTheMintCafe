using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class PartyTable
{
    public int TableId { get; set; }

    public string? Status { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
