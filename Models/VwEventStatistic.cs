using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Team1_Code_Stars_Events.Models;

[Keyless]
public partial class VwEventStatistic
{
    public int EventId { get; set; }

    [StringLength(150)]
    public string EventTitle { get; set; } = null!;

    public int? TicketsSold { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal? TotalRevenue { get; set; }

    public int? MaxCapacity { get; set; }
}
