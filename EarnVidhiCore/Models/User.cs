using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EarnVidhiCore.Models;

[Table("User")]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserName { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? UserPromo { get; set; }

    [StringLength(10)]
    public string? UserMobile { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserEmail { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserPassword { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserStatus { get; set; }

    public int? UserSponsor { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UserRegistered { get; set; }

    public int? UserEmailVerify { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? MainWallet { get; set; }
}
