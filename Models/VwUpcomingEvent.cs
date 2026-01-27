using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Team1_Code_Stars_Events.Models;

[Keyless]
public partial class VwUpcomingEvent
{
    // LÄGG TILL DENNA RAD:
    public int EventId { get; set; }

    [StringLength(150)]
    public string Event { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [StringLength(100)]
    public string Venue { get; set; } = null!;

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string Organizer { get; set; } = null!;
}
