using System;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<User> Users {get;set;}
    public DbSet<FileEntity> Files {get;set;}
    public DbSet<RefreshToken> RefreshTokens {get;set;}
    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.Entity<User>(entity =>
        {
            entity.HasKey(m=>m.Id);
            entity.Property(m=>m.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(m=>m.Email).IsUnique();
        });
    }
}
