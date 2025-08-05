namespace EmployeeManagementProject.Application_Layer.Common
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository EmployeeRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}