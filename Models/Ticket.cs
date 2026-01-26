using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Team1_Code_Stars_Events.Models;

public partial class Ticket
{
    [Key]
    public int TicketId { get; set; }

    public int EventId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PurchaseDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [StringLength(20)]
    public string? Type { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Tickets")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("EventId")]
    [InverseProperty("Tickets")]
    public virtual Event Event { get; set; } = null!;
}
