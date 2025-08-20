using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Domain_Layer.Events;
using FluentValidation;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class CreateEmployeeHandlers : IRequestHandler<CreateEmployeeCommand, Employee>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IValidator<Employee> _employeeValidator;

        public CreateEmployeeHandlers(IUnitOfWork unitOfWork, IMediator mediator, IValidator<Employee> employeeValidator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _employeeValidator = employeeValidator;
        }

        public async Task<Employee> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                Employee employee = Employee.Create(request.Name, request.Address, request.Email, request.Phone);

                var validationResult = await _employeeValidator.ValidateAsync(employee);

                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                await _unitOfWork.EmployeeRepository.AddEmployeeAsync(employee);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                await _mediator.Publish(new EmployeeCreatedEvent(employee), cancellationToken);

                return employee;
            }
            catch
            {
                await _unitOfWork.AbortTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}