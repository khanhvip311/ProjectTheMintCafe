using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class BookingDetail
{
    public int BookingDetailId { get; set; }

    public int? BookingId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Product? Product { get; set; }
}
