using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public DateTime BookingTime { get; set; }

    public bool Status { get; set; }

    public int TableId { get; set; }

    public int PaymentMethod { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Voucher { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual PartyTable Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Voucher? VoucherNavigation { get; set; }
}
