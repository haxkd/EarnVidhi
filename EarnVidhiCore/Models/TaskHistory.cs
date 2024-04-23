using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EarnVidhiCore.Models;

[Table("TaskHistory")]
public partial class TaskHistory
{
    [Key]
    public int HistoryId { get; set; }

    public int? UserId { get; set; }

    public string? TaskToken { get; set; }
    public int? TaskId { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
}
