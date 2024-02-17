using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class Notification
{
    [Key]
    [Column("NotificationID")]
    public int NotificationId { get; set; }

    [Column("UserID")]
    public int? UserId { get; set; }

    public int NotificationType { get; set; }

    [Column("SourceID")]
    public int? SourceId { get; set; }

    public bool IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NotificationDate { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User? User { get; set; }
}
