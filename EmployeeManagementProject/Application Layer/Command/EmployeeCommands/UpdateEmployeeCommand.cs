using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class UpdateEmployeeCommand : IRequest<int>
    {
        public UpdateEmployeeCommand(int id, EmployeeName name, string address, string email, string phone)
        {
            Id = id;
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
        }

        public int Id { get; set; }

        public EmployeeName Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}