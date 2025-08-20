using EmployeeManagementProject.Domain_Layer.Entities;

namespace EmployeeManagementProject.Application_Layer.Common
{
    public interface IEmployeeRepository
    {
        public Task AddEmployeeAsync(Employee employee);

        public Task<int> DeleteEmployeeAsync(string id);

        public Task UpdateEmployee(Employee employee);

        public Task<Employee> GetEmployeeByIdAsync(string id);

        public Task<List<Employee>> GetEmployeeListAsync();
    }
}