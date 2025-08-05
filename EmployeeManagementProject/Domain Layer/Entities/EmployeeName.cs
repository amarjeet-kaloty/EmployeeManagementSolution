using System.Text.Json.Serialization;

namespace EmployeeManagementProject.Domain_Layer.Entities
{
    public class EmployeeName
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; }

        public EmployeeName() : base()
        {
        }

        [JsonConstructor]
        public EmployeeName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Employee name cannot be null or empty.", nameof(fullName));
            }

            if (fullName.Length > 50)
            {
                throw new ArgumentException("Employee name cannot exceed 50 characters.", nameof(fullName));
            }

            FullName = fullName;
        }
    }
}