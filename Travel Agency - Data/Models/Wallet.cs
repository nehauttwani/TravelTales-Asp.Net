using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models;

public partial class Wallet
{
    [Key]
    public int WalletId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Wallets")]
    public virtual Customer Customer { get; set; } = null!;
}
