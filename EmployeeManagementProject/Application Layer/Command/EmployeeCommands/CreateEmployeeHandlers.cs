using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Application_Layer.Validation;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Domain_Layer.Events;
using FluentValidation;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class CreateEmployeeHandlers : IRequestHandler<CreateEmployeeCommand, Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IValidator<Employee> _employeeValidator;

        public CreateEmployeeHandlers(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, IMediator mediator, IValidator<Employee> employeeValidator)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _employeeValidator = employeeValidator;
        }

        public async Task<Employee> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
           
            Employee employee = Employee.Create(request.Name, request.Address, request.Email, request.Phone);
            
            var validationResult = await _employeeValidator.ValidateAsync(employee);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _employeeRepository.AddEmployeeAsync(employee);

            await _mediator.Publish(new EmployeeCreatedEvent(employee), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return employee;
        }
    }
}