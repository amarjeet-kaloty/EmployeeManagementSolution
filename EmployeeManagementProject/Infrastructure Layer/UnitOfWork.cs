using EmployeeManagementProject.Application_Layer.Common;
using MongoDB.Driver;

namespace EmployeeManagementProject.Infrastructure_Layer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _mongoClient;
        private IClientSessionHandle _session;
        private IEmployeeRepository _employeeRepository;

        public UnitOfWork(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                if (_session == null)
                {
                    throw new InvalidOperationException("Session has not been started. Call BeginTransactionAsync() first.");
                }

                if (_employeeRepository == null)
                {
                    _employeeRepository = new EmployeeRepository(_session, _mongoClient.GetDatabase("EmployeeManagementDB"));
                }
                return _employeeRepository;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
            _session.StartTransaction();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _session.CommitTransactionAsync(cancellationToken);
            }
            catch (MongoException)
            {
                await _session.AbortTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _session.Dispose();
            }
        }

        public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _session.AbortTransactionAsync(cancellationToken);
            _session.Dispose();
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}