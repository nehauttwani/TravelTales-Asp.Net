using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Agency___Data.Models;

public partial class Purchase
{
    [Key]
    public int PurchaseId { get; set; }

    public int CustomerId { get; set; }

    public int PackageId { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PurchaseDate { get; set; }

    public bool IsPaid { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Purchases")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("PackageId")]
    [InverseProperty("Purchases")]
    public virtual Package Package { get; set; } = null!;
}
