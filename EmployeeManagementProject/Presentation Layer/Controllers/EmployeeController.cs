using EmployeeManagementProject.Application_Layer.Command.EmployeeCommands;
using EmployeeManagementProject.Application_Layer.Query.EmployeeQueries;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Presentation_Layer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementProject.Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// Adds an employee to the system.
        /// </summary>
        /// <param name="employeeDto">The data transfer object containing the details of the new employee to be added.</param>
        /// <returns>
        /// The newly created object, including its assigned ID.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee([FromBody] CreateEmployeeDTO employeeDto)
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand(
                employeeDto.Name,
                employeeDto.Address,
                employeeDto.Email,
                employeeDto.Phone!
            );

            Employee newEmployee = await _mediator.Send(command);

            if (newEmployee == null)
            {
                return StatusCode(500, "Failed to create employee. An unexpected error occurred.");
            }

            return Ok(newEmployee);
        }

        /// <summary>
        /// Updates the existing employee in the system.
        /// </summary>
        /// <param name="employeeDto">The data transfer object containing the details of the employee to be updated.</param>
        /// <returns>
        /// An integer representing the number of rows affected.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDTO employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmployeeName employeeName = new EmployeeName(employeeDto.Name);
            var command = new UpdateEmployeeCommand(
                id,
                employeeName,
                employeeDto.Address,
                employeeDto.Email,
                employeeDto.Phone
            );

            int updatedEmployee = await _mediator.Send(command);

            if (updatedEmployee == 0)
            {
                return NotFound($"Employee with ID {id} not found for update.");
            }

            return Ok(updatedEmployee);
        }

        /// <summary>
        /// Deletes an employee in the system.
        /// </summary>
        /// <param name="id">The unique integer identifier of the employee to delete. Must be a positive integer.</param>
        /// <returns>
        /// An integer representing the number of rows affected (typically 1 for successful deletion).
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteEmployee(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Employee ID must be a positive integer.");
            }

            int employeeDeletedCount = await _mediator.Send(new DeleteEmployeeCommand() { Id = id });

            if (employeeDeletedCount == 0)
            {
                return NotFound($"Employee with ID {id} not found for deletion.");
            }

            return Ok(id);
        }

        /// <summary>
        /// Retrieves a list of all employees.
        /// </summary>
        /// <returns>
        /// A List of Employees or an empty list if no employees are found.
        /// </returns>
        [HttpGet]
        public async Task<List<Employee>> GetEmployeeList()
        {
            var employeeList = await _mediator.Send(new GetEmployeeListQuery());
            return employeeList;
        }

        /// <summary>
        /// Retrieves a specific employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique integer identifier of the employee to retrieve. Must be a positive integer.</param>
        /// <returns>
        /// An employee object corresponding to the provided unique identifier, if one exists.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Employee ID must be a positive integer.");
            }

            Employee employee = await _mediator.Send(new GetEmployeeByIdQuery() { Id = id });

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            return Ok(employee);
        }
    }
}