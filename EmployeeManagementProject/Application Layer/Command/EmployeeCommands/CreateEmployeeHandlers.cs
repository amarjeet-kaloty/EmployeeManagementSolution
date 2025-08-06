using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Domain_Layer.Events;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class CreateEmployeeHandlers : IRequestHandler<CreateEmployeeCommand, Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateEmployeeHandlers(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, IMediator mediator)
        {
            this._employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Employee> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
           
            Employee employee = Employee.Create(request.Name.ToString(), request.Address, request.Email, request.Phone);

            await _employeeRepository.AddEmployeeAsync(employee);

            await _mediator.Publish(new EmployeeCreatedEvent(employee), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return employee;
        }
    }
}