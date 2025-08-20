using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class DeleteEmployeeCommand : IRequest<int>
    {
        public string Id { get; set; }
    }
}