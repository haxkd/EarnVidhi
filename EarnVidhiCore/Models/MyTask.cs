using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EarnVidhiCore.Models;

[Table("Tasks")]
public partial class MyTask
{
    [Key]
    public int TaskId { get; set; }

    [Unicode(false)]
    public string? TaskUrl { get; set; }

    public int? TaskStatus { get; set; }
}
