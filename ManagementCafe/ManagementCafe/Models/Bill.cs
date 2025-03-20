using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool? Status { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual User? User { get; set; }
}
