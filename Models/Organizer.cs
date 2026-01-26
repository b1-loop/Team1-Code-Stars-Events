using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Team1_Code_Stars_Events.Models;

[Index("Email", Name = "UQ__Organize__A9D10534AA8BFB8B", IsUnique = true)]
public partial class Organizer
{
    [Key]
    public int OrganizerId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Organizer")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
