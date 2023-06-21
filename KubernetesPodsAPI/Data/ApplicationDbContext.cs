using Microsoft.EntityFrameworkCore;
using KubernetesPodsAPI.Models;

namespace KubernetesPodsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Cluster> Clusters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<ConfigFile> ConfigFiles { get; set; }
        public DbSet<Pod> Pods { get; set; }
    }
}
