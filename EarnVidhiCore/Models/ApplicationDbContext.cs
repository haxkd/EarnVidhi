using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EarnVidhiCore.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-V0FFFT4;Initial Catalog=db_earnvidhi;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.Property(e => e.MainWallet).HasDefaultValue(0m);
        //    entity.Property(e => e.UserEmailVerify).HasDefaultValue(0);
        //    entity.Property(e => e.UserMobile).IsFixedLength();
        //    entity.Property(e => e.UserRegistered).HasDefaultValueSql("(getdate())");
        //    entity.Property(e => e.UserSponsor).HasDefaultValue(0);
        //    entity.Property(e => e.UserStatus).HasDefaultValueSql("((0))");
        //});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
