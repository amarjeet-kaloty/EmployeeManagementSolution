using EmployeeManagementProject.Domain_Layer.Entities;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.Query.EmployeeQueries
{
    public class GetEmployeeListQuery : IRequest<List<Employee>>
    {
    }
}