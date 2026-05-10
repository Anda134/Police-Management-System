using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Data
{
    /// <summary>Main database context for the Police Management System.</summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PoliceStation> PoliceStations { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<CriminalFile> CriminalFiles { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<AgentTransfer> AgentTransfers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CriminalFileHistory> CriminalFileHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Agent>()
                .HasOne(a => a.PoliceStation)
                .WithMany(ps => ps.Agents)
                .HasForeignKey(a => a.PoliceStationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Agent>()
                .HasOne(a => a.Superior)
                .WithMany(a => a.Subordinates)
                .HasForeignKey(a => a.SuperiorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CriminalFile>()
                .HasOne(cf => cf.PoliceStation)
                .WithMany()
                .HasForeignKey(cf => cf.PoliceStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CriminalFile>()
                .HasOne(cf => cf.Agent)
                .WithMany()
                .HasForeignKey(cf => cf.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conference>()
                .HasOne(c => c.Organizer)
                .WithMany()
                .HasForeignKey(c => c.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conference>()
                .HasMany(c => c.Participants)
                .WithMany();

            modelBuilder.Entity<AgentTransfer>()
                .HasOne(at => at.Agent)
                .WithMany()
                .HasForeignKey(at => at.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AgentTransfer>()
                .HasOne(at => at.FromStation)
                .WithMany()
                .HasForeignKey(at => at.FromStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AgentTransfer>()
                .HasOne(at => at.ToStation)
                .WithMany()
                .HasForeignKey(at => at.ToStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Agent)
                .WithMany()
                .HasForeignKey(u => u.AgentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.SetNull);

           modelBuilder.Entity<CriminalFileHistory>()
                .HasOne(cfh => cfh.CriminalFile)
                .WithMany()
                .HasForeignKey(cfh => cfh.CriminalFileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}