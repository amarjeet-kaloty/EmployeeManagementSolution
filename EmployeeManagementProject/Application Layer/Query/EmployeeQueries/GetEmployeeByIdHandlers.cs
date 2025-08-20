using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Query.EmployeeQueries
{
    public class GetEmployeeByIdHandlers : IRequestHandler<GetEmployeeByIdQuery, Employee>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEmployeeByIdHandlers(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Employee> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                return await _unitOfWork.EmployeeRepository.GetEmployeeByIdAsync(request.Id);
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