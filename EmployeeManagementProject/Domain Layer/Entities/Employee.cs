using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementProject.Domain_Layer.Entities
{
    public class Employee : Entity
    {
        [Required(ErrorMessage = "Name is required.")]
        public EmployeeName Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public string? Phone { get; set; }

        public Employee() : base()
        {
        }

        public Employee(EmployeeName name, string address, string email, string? phone) : base()
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
        }
    }
}