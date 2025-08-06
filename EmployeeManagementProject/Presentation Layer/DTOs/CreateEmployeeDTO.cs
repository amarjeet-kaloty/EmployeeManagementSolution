using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementProject.Presentation_Layer.DTOs
{
    public class CreateEmployeeDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        public string? Phone { get; set; }
    }
}
