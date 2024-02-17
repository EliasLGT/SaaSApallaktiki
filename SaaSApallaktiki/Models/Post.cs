using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class Post
{
    [Key]
    [Column("PostID")]
    public int PostId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    [Column("HobbyID")]
    public int? HobbyId { get; set; }

    [Column("CourseID")]
    public int? CourseId { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string? PostTitle { get; set; }

    [Unicode(false)]
    public string PostContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime PostDate { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Posts")]
    public virtual Course? Course { get; set; }

    [ForeignKey("HobbyId")]
    [InverseProperty("Posts")]
    public virtual Hobby? Hobby { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Posts")]
    public virtual User? User { get; set; }
}
