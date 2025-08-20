using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class UpdateEmployeeHandlers : IRequestHandler<UpdateEmployeeCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEmployeeHandlers(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                Employee employee = await _unitOfWork.EmployeeRepository.GetEmployeeByIdAsync(request.Id);

                if (employee == null)
                    return $"Employee with Id {request.Id} not found.";

                employee.UpdateDetails(request.Name!, request.Address!, request.Email!, request.Phone);

                await _unitOfWork.EmployeeRepository.UpdateEmployee(employee);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return employee.Id;
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