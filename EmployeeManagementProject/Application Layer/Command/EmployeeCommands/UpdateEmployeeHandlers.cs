using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class UpdateEmployeeHandlers : IRequestHandler<UpdateEmployeeCommand, int>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeHandlers(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        {
            this._employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            Employee employee = await _employeeRepository.GetEmployeeByIdAsync(request.Id);
            if (employee == null)
                return 0;
            employee.Name = request.Name;
            employee.Address = request.Address;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            _employeeRepository.UpdateEmployee(employee);

            int affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return affectedRows;
        }
    }
}