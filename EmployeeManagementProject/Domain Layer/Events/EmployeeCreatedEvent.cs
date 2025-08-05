using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Domain_Layer.Events
{
    public class EmployeeCreatedEvent : INotification
    {
        public Employee _employee { get; set; }

        public EmployeeCreatedEvent(Employee employee)
        {
            _employee = employee;
        }
    }
}