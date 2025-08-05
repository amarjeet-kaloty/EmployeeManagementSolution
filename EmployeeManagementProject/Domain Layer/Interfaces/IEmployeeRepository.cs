using EmployeeManagementProject.Domain_Layer.Entities;

namespace EmployeeManagementProject.Application_Layer.Common
{
    public interface IEmployeeRepository
    {
        public Task AddEmployeeAsync(Employee employee);

        public Task<int> DeleteEmployeeAsync(int id);

        public Task<Employee> GetEmployeeByIdAsync(int id);

        public Task<List<Employee>> GetEmployeeListAsync();

        public void UpdateEmployee(Employee employee);
    }
}