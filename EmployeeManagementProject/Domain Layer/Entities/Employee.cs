namespace EmployeeManagementProject.Domain_Layer.Entities
{
    public class Employee : Entity
    {
        public EmployeeName Name { get; private set; }

        public string Address { get; private set; }

        public string Email { get; private set; }

        public string? Phone { get; private set; }

        public Employee() : base()
        {
        }

        public Employee(string id, EmployeeName name, string address, string email, string? phone) : base(id)
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
        }

        public static Employee Create(string name, string address, string email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address is required.", nameof(address));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            EmployeeName employeeName = new EmployeeName(name);
            Employee employee = new Employee();
            employee.Name = employeeName;
            employee.Address = address;
            employee.Email = email;
            employee.Phone = phone;

            return employee;
        }

        public void UpdateDetails(EmployeeName name, string address, string email, string? phone)
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
        }
    }
}