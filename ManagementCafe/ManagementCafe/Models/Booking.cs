using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public DateTime? BookingTime { get; set; }

    public bool? Status { get; set; }

    public int? NumGuests { get; set; }

    public string? Note { get; set; }

    public int? TableId { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual PartyTable? Table { get; set; }

    public virtual User? User { get; set; }
}
