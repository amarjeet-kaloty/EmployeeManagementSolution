using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementProject.Presentation_Layer.DTOs
{
    public class UpdateEmployeeDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}