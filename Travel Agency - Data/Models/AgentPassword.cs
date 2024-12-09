using System;
using System.Collections.Generic;

namespace Travel_Agency___Data.Models;

public partial class AgentPassword
{
    public int AgentId { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public virtual Agent Agent { get; set; } = null!;
}
