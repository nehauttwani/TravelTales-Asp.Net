using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models;

public partial class AgentPassword
{
    [Key]
    [Column("AgentID")]
    public int AgentId { get; set; }

    [MaxLength(64)]
    public byte[] PasswordHash { get; set; } = null!;

    [Column("isAdmin")]
    public bool IsAdmin { get; set; }

    [ForeignKey("AgentId")]
    [InverseProperty("AgentPassword")]
    public virtual Agent Agent { get; set; } = null!;
}
