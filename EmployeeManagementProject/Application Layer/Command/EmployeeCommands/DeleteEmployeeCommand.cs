using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class DeleteEmployeeCommand : IRequest<int>
    {
        public int Id { get; set; }
    }
}