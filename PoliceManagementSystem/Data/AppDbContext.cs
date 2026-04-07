using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Data
{
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
    }
}