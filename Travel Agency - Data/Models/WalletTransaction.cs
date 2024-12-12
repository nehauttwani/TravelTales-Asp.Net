using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models;

public partial class WalletTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransactionDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string TransactionType { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("WalletTransactions")]
    public virtual Customer Customer { get; set; } = null!;
}
