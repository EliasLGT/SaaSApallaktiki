using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class FriendRequest
{
    [Key]
    [Column("RequestID")]
    public int RequestId { get; set; }

    [Column("SenderID")]
    public int? SenderId { get; set; }

    [Column("ReceiverID")]
    public int? ReceiverId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RequestDate { get; set; } = DateTime.Now;

    public int Status { get; set; }

    [ForeignKey("ReceiverId")]
    [InverseProperty("FriendRequestReceivers")]
    public virtual User? Receiver { get; set; }

    [ForeignKey("SenderId")]
    [InverseProperty("FriendRequestSenders")]
    public virtual User? Sender { get; set; }
}
