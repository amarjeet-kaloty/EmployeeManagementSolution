

namespace EmployeeManagementProject.Presentation_Layer.DTOs
{
    public class CreateEmployeeDTO
    {
        public required string Name { get; set; }

        public required string Address { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }
    }
}
