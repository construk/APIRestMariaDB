using Microsoft.EntityFrameworkCore;
using PruebaApi.Models;

namespace PruebaApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PruebaApi.Models.Test> Test { get; set; }

        public virtual DbSet<Test> TestList { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>(entity =>
            {
                entity.ToTable("test");
                entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("int(11)")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");
                entity.Property(e=>e.Nombre)
                .HasColumnName("nombre")
                .HasColumnType("varchar(20)")
                .HasDefaultValue("'NULL'")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");
            });
    }
    }
}
