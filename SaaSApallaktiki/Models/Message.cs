using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class Message
{
    [Key]
    [Column("MessageID")]
    public int MessageId { get; set; }

    [Column("SenderID")]
    public int? SenderId { get; set; }

    [Column("RecipientID")]
    public int? RecipientId { get; set; }

    [Unicode(false)]
    public string MessageContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime SentDate { get; set; }

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User? Sender { get; set; }
}
