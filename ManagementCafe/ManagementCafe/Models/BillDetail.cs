using System;
using System.Collections.Generic;

namespace ManagementCafe.Models;

public partial class BillDetail
{
    public int BillDetailId { get; set; }

    public int? BillId { get; set; }

    public int? ProductId { get; set; }

    public string? Note { get; set; }

    public virtual Bill? Bill { get; set; }

    public virtual Product? Product { get; set; }
}
