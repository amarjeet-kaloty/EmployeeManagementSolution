using EmployeeManagementProject.Domain_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeManagementProject.Infrastructure_Layer
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Name)
                      .HasColumnType("varchar(50)")
                      .HasConversion(new ValueConverter<EmployeeName, string>(
                          v => v.FullName,
                          v => new EmployeeName(v)));

                entity.Property(e => e.Address)
                      .HasColumnType("text");

                entity.Property(e => e.Email)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.Phone)
                      .HasColumnType("varchar(20)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}