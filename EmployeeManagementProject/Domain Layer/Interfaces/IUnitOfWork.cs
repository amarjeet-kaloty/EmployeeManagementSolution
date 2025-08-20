namespace EmployeeManagementProject.Application_Layer.Common
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository EmployeeRepository { get; }

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    }
}