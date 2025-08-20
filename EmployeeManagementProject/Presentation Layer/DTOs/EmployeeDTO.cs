using EmployeeManagementProject.Domain_Layer.Entities;

namespace EmployeeManagementProject.Presentation_Layer.DTOs
{
    public class EmployeeDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }

        public static EmployeeDTO FromEmployee(Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name.ToString(),
                Address = employee.Address,
                Email = employee.Email,
                Phone = employee.Phone
            };
        }
    }
}
