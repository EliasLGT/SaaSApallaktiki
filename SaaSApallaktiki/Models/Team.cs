using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class Team
{
    [Key]
    [Column("TeamID")]
    public int TeamId { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string TeamName { get; set; } = null!;

    [InverseProperty("Team")]
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
