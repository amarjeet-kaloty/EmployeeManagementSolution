using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using MongoDB.Driver;

namespace EmployeeManagementProject.Infrastructure_Layer
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IClientSessionHandle _session;
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeRepository(IClientSessionHandle session, IMongoDatabase database)
        {
            _session = session;
            _employees = database.GetCollection<Employee>("employeesCollection");
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _employees.InsertOneAsync(employee);
        }

        public async Task<int> DeleteEmployeeAsync(string id)
        {
            var employeeToDelete = await _employees.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (employeeToDelete == null)
            {
                return 0;
            }

            await _employees.DeleteOneAsync(emp => emp.Id == employeeToDelete.Id);

            return 1;
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            return await _employees.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Employee>> GetEmployeeListAsync()
        {
            return _employees.Find(_ => true).ToList();
        }

        public async Task UpdateEmployee(Employee employee)
        {
            await _employees.ReplaceOneAsync(x => x.Id == employee.Id, employee);
        }
    }
}