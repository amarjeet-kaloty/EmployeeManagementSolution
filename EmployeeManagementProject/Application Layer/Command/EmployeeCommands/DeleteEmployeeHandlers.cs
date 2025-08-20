using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Command.EmployeeCommands
{
    public class DeleteEmployeeHandlers : IRequestHandler<DeleteEmployeeCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEmployeeHandlers(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                Employee employee = await _unitOfWork.EmployeeRepository.GetEmployeeByIdAsync(request.Id);
                if (employee == null)
                {
                    return 0;
                }

                await _unitOfWork.EmployeeRepository.DeleteEmployeeAsync(request.Id);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return 1;
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