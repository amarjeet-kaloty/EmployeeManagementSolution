using EmployeeManagementProject.Application_Layer.Common;
using EmployeeManagementProject.Domain_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementProject.Infrastructure_Layer
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _dbContext;

        public EmployeeRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _dbContext.Employees.AddAsync(employee);
        }

        public async Task<int> DeleteEmployeeAsync(int id)
        {
            var employeeToDelete = await _dbContext.Employees.FindAsync(id);

            if (employeeToDelete == null)
            {
                return 0;
            }

            _dbContext.Employees.Remove(employeeToDelete);

            return 1;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _dbContext.Employees.FindAsync(id);
        }

        public async Task<List<Employee>> GetEmployeeListAsync()
        {
            List<Employee> employees = await _dbContext.Employees.ToListAsync();
            return employees;
        }

        public void UpdateEmployee(Employee employee)
        {
            _dbContext.Employees.Update(employee);
        }
    }
}