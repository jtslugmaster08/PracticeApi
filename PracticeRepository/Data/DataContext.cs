 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PracticeModel.Entities;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeRepository.Data
{
    public class DataContext:IdentityDbContext<BaseUser>
    {
        private readonly IConfiguration _configuration;
        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Weather> Weather { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Weather>().Property(o=>o.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Error>().Property(o=>o.CreatedDate).HasDefaultValue(DateTime.Now);
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public Func<DateTime> TimestampProvider { get; set; }=() => DateTime.UtcNow;

        public override int SaveChanges()
        {
            TrackChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void TrackChanges()
        {
            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity is BaseClass)
                {
                    var auditable = entry.Entity as BaseClass;
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedDate = TimestampProvider();
                        auditable.UpdatedDate = TimestampProvider();
                    }
                    else
                    {
                        auditable.UpdatedDate = TimestampProvider();
                    }
                }
            }
        }
    }
}
