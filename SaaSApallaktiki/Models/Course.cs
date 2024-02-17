using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

[Index("CourseName", Name = "UQ__Courses__9526E277EF8C241F", IsUnique = true)]
public partial class Course
{
    [Key]
    [Column("CourseID")]
    public int CourseId { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string CourseName { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [ForeignKey("CourseId")]
    [InverseProperty("Courses")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
