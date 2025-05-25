using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestaoResiduos.API.Models;

namespace GestaoResiduos.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Residue> Residues { get; set; } = null!;
        public DbSet<CollectionPoint> CollectionPoints { get; set; } = null!;
        public DbSet<ScheduledCollection> ScheduledCollections { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<ScheduledCollection>()
                .HasOne(sc => sc.Residue)
                .WithMany()
                .HasForeignKey(sc => sc.ResidueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduledCollection>()
                .HasOne(sc => sc.CollectionPoint)
                .WithMany()
                .HasForeignKey(sc => sc.CollectionPointId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Residue)
                .WithMany()
                .HasForeignKey(n => n.ResidueId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.CollectionPoint)
                .WithMany()
                .HasForeignKey(n => n.CollectionPointId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Set default value for CreatedAt fields
            modelBuilder.Entity<Residue>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<CollectionPoint>()
                .Property(cp => cp.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
                
            modelBuilder.Entity<ScheduledCollection>()
                .Property(sc => sc.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
                
            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}