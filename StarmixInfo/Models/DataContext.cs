using System;
using Microsoft.EntityFrameworkCore;
using StarmixInfo.Models.Data;

namespace StarmixInfo.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Config> Config { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}
