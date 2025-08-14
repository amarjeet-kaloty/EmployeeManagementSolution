using EmployeeManagementProject.Domain_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeManagementProject.Infrastructure_Layer
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder) 
        {
            builder.Property(e => e.Name)
                .HasColumnType("varchar(50)")
                .HasConversion(new ValueConverter<EmployeeName, string>
                    (
                        v => v.FullName,
                        v => new EmployeeName(v)
                    )
                )
                .IsRequired();

            builder.Property(e => e.Address)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.Email)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(e => e.Phone)
                .HasColumnType("varchar(12)");
        }
    }
}
