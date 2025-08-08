using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class CreateEmployeeCommand : IRequest<Employee>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public CreateEmployeeCommand(string name, string address, string email, string phone)
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
        }
    }
}