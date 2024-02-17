using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

[PrimaryKey("TeamId", "UserId")]
public partial class TeamMember
{
    [Key]
    [Column("TeamID")]
    public int TeamId { get; set; }

    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    public int Status { get; set; }

    [ForeignKey("TeamId")]
    [InverseProperty("TeamMembers")]
    public virtual Team? Team { get; set; }// = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TeamMembers")]
    public virtual User? User { get; set; }// = null!;
}
