using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementProject.Presentation_Layer.DTOs
{
    public class UpdateEmployeeDTO
    {
        public required string Name { get; set; }

        public required string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        public required string Phone { get; set; }
    }
}
