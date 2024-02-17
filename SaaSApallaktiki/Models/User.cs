using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

[Index("Username", Name = "UQ__Users__536C85E450A05F02", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(32)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(32)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [InverseProperty("Receiver")]
    public virtual ICollection<FriendRequest> FriendRequestReceivers { get; set; } = new List<FriendRequest>();

    [InverseProperty("Sender")]
    public virtual ICollection<FriendRequest> FriendRequestSenders { get; set; } = new List<FriendRequest>();

    [InverseProperty("Sender")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("User")]
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
}
