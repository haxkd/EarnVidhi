using System;
using System.Collections.Generic;

namespace EarnVidhiCore.Models;

public partial class EmailVerify
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? VerifyCode { get; set; }

    public DateTime? VerifyDate { get; set; }

    public virtual User? User { get; set; }
}
