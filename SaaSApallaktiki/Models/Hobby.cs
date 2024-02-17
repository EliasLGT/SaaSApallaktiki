using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

[Index("HobbyName", Name = "UQ__Hobbies__C3928600C5C3BF1D", IsUnique = true)]
public partial class Hobby
{
    [Key]
    [Column("HobbyID")]
    public int HobbyId { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string HobbyName { get; set; } = null!;

    [InverseProperty("Hobby")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [ForeignKey("HobbyId")]
    [InverseProperty("Hobbies")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
