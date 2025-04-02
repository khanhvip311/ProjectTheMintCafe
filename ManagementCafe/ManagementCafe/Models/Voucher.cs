using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class Voucher
{
    public string VoucherId { get; set; } = null!;

    public int Value { get; set; }

    public string? Description { get; set; }

    public bool State { get; set; }

    public bool IsPercentage { get; set; }

    public int Requirement { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
