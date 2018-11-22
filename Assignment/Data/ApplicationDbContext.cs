using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Assignment.Data;

namespace Assignment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity=> 
            {
                entity.HasKey(e => e.UserId);

            });

            modelBuilder.Entity<UserUrl>(entity =>
            {
                entity.HasKey(e => e.UrlId);

                entity.HasOne<User>().WithMany().HasForeignKey(f=>f.UserId);
            });
        }

        //entities
        public DbSet<User> Users { get; set; }

        //entities
        public DbSet<Assignment.Data.UserUrl> UserUrl { get; set; }
    }
}
