using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WeWorkDotnet.Web.Models;

namespace WeWorkDotnet.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ContractType>().ToTable("ContractTypes");

            builder.Entity<Job>().ToTable("Jobs");

            builder.Entity<Job>()
                .HasOne(o => o.ContractType)
                .WithMany()
                .HasForeignKey(fk => fk.ContractTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Job>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(fk => fk.PostedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Job> Job { get; set; }
    }
}
