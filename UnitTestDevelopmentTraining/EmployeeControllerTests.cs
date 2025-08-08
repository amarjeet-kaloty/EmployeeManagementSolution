using EmployeeManagementProject.Application_Layer.Command.EmployeeCommands;
using EmployeeManagementProject.Application_Layer.Query.EmployeeQueries;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Presentation_Layer.Controllers;
using EmployeeManagementProject.Presentation_Layer.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace UnitTestDevelopmentTraining
{
    public class EmployeeControllerTests
    {
        private readonly IMediator _mediator;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new EmployeeController(_mediator);
        }

        [Fact]
        public async Task GetEmployeeList_ReturnListOfAllEmployees_SuccessAsync()
        {
            // Arrange
            var expectedEmployees = new List<Employee>
            {
                Employee.Create(
                    name: "Test Employee1",
                    address: "123 Praline Ave",
                    email: "employee1@gmail.com",
                    phone: "404-111-1234"
                ),

                Employee.Create(
                    name: "Test Employee2",
                    address: "456 Orange Lane",
                    email: "employee2@gmail.com",
                    phone: "505-000-7896"
                )
            };

            _mediator.Send(Arg.Any<GetEmployeeListQuery>()).Returns(expectedEmployees);

            // Act
            var result = await _controller.GetEmployeeList();

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetEmployeeListQuery>());

            Assert.NotNull(result);
            Assert.Equal(expectedEmployees.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(expectedEmployees[i].Id, result[i].Id);
                Assert.Equal(expectedEmployees[i].Name, result[i].Name);
                Assert.Equal(expectedEmployees[i].Address, result[i].Address);
                Assert.Equal(expectedEmployees[i].Email, result[i].Email);
                Assert.Equal(expectedEmployees[i].Phone, result[i].Phone);
            }
        }

        [Fact]
        public async Task GetEmployeeList_ReturnsEmptyList_WhenNoEmployeesExist()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetEmployeeListQuery>()).Returns(new List<Employee>());

            // Act
            var result = await _controller.GetEmployeeList();

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetEmployeeListQuery>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsEmployee()
        {
            // Arrange
            EmployeeName employeeName = new EmployeeName("Test Employee1");
            Employee expectedEmployee = new Employee(
                id: 1,
                name: employeeName,
                address: "123 Praline Ave",
                email: "employee1@gmail.com",
                phone: "404-111-1234"
            );

            _mediator.Send(Arg.Any<GetEmployeeByIdQuery>()).Returns(expectedEmployee);

            // Act
            var result = await _controller.GetEmployee(expectedEmployee.Id);

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetEmployeeByIdQuery>());
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(expectedEmployee.Id, actualEmployee.Id);
            Assert.Equal(expectedEmployee.Name, actualEmployee.Name);
            Assert.Equal(expectedEmployee.Address, actualEmployee.Address);
            Assert.Equal(expectedEmployee.Email, actualEmployee.Email);
            Assert.Equal(expectedEmployee.Phone, actualEmployee.Phone);
        }

        [Fact]
        public async Task GetEmployeeById_InValidId_FailsToReturnEmployee()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetEmployeeByIdQuery>()).Returns(Task.FromResult<Employee>(null));

            var nonExistentId = 101;

            // Act
            var result = await _controller.GetEmployee(nonExistentId);

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetEmployeeByIdQuery>());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Employee with ID {nonExistentId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task AddEmployee_ValidEmployee_ReturnsCreated()
        {
            // Arrange
            CreateEmployeeDTO employeeDto = new CreateEmployeeDTO
            {
                Name = "Test Employee 1",
                Address = "123 Praline Ave",
                Email = "employee1@gmail.com",
                Phone = "404-111-1234"
            };

            Employee newEmployee = new Employee
            (
                id: 1,
                name: new EmployeeName(employeeDto.Name),
                address: employeeDto.Address,
                email: employeeDto.Email,
                phone: employeeDto.Phone
            );

            _mediator.Send(Arg.Is<CreateEmployeeCommand>(cmd =>
                cmd.Name == employeeDto.Name &&
                cmd.Address == employeeDto.Address &&
                cmd.Email == employeeDto.Email))
            .Returns(newEmployee);

            // Act
            var result = await _controller.AddEmployee(employeeDto);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<CreateEmployeeCommand>(cmd =>
             cmd.Name == employeeDto.Name &&
             cmd.Address == employeeDto.Address &&
             cmd.Email == employeeDto.Email));

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualEmployee = Assert.IsType<Employee>(okResult.Value);

            Assert.Equal(newEmployee.Id, actualEmployee.Id);
            Assert.Equal(newEmployee.Name, actualEmployee.Name);
            Assert.Equal(newEmployee.Address, actualEmployee.Address);
            Assert.Equal(newEmployee.Email, actualEmployee.Email);
            Assert.Equal(newEmployee.Phone, actualEmployee.Phone);
        }

        [Fact]
        public async Task AddEmployee_InValidEmployeeModel_ReturnsBadRequest()
        {
            // Arrange
            CreateEmployeeDTO invalidEmployeeDto = new CreateEmployeeDTO
            {
                Name = "Test Employee 1",
                Address = "123 Praline Ave",
                Email = null,
                Phone = "404-111-1234"
            };

            _controller.ModelState.AddModelError(nameof(Employee.Email), "Invalid email format.");

            // Act
            var result = await _controller.AddEmployee(invalidEmployeeDto);

            // Assert
            await _mediator.DidNotReceive().Send(Arg.Any<CreateEmployeeCommand>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey(nameof(Employee.Email)));
            Assert.Contains("Invalid email format.", modelState[nameof(Employee.Email)] as string[]);
        }

        [Fact]
        public async Task AddEmployee_MediatorReturnsNull_ReturnsInternalServerError()
        {
            // Arrange
            CreateEmployeeDTO employeeDto = new CreateEmployeeDTO
            {
                Name = "Test Employee 1",
                Address = "123 Praline Ave",
                Email = "employee1@gmail.com",
                Phone = "404-111-1234"
            };

            Employee validEmployee = new Employee
            (
                id: 1,
                name: new EmployeeName(employeeDto.Name),
                address: employeeDto.Address,
                email: employeeDto.Email,
                phone: employeeDto.Phone
            );

            _mediator.Send(Arg.Any<CreateEmployeeCommand>()).Returns(Task.FromResult<Employee>(null));

            // Act
            var result = await _controller.AddEmployee(employeeDto);

            // Assert
            await _mediator.Received(1).Send(Arg.Any<CreateEmployeeCommand>());
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Failed to create employee. An unexpected error occurred.", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateEmployee_ValidEmployee_ReturnsOkWithUpdatedId()
        {
            // Arrange
            int employeeIdToUpdate = 1;

            UpdateEmployeeDTO updateEmployeeDTO = new UpdateEmployeeDTO
            {
                Name = "Update Name",
                Address = "Updated Address",
                Email = "updated@gmail.com",
                Phone = "404-111-1234"
            };

            _mediator.Send(Arg.Any<UpdateEmployeeCommand>()).Returns(Task.FromResult(employeeIdToUpdate));

            // Act
            var result = await _controller.UpdateEmployee(employeeIdToUpdate, updateEmployeeDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualUpdatedId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(employeeIdToUpdate, actualUpdatedId);
        }

        [Fact]
        public async Task UpdateEmployee_InValidEmployee_ThrowsException()
        {
            // Arrange
            int employeeIdToUpdate = 1;

            UpdateEmployeeDTO updateEmployeeDTO = new UpdateEmployeeDTO
            {
                Name = null,
                Address = "Updated Address",
                Email = null,
                Phone = "404-111-1234"
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _controller.UpdateEmployee(employeeIdToUpdate, updateEmployeeDTO);
            });
        }

        [Fact]
        public async Task UpdateEmployee_EmployeeNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int nonExistentEmployeeId = 999;
            UpdateEmployeeDTO updateEmployeeDTO = new UpdateEmployeeDTO
            {
                Name = "Update Name",
                Address = "Updated Address",
                Email = null,
                Phone = "404-111-1234"
            };

            _mediator.Send(Arg.Any<UpdateEmployeeCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await _controller.UpdateEmployee(nonExistentEmployeeId, updateEmployeeDTO);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<UpdateEmployeeCommand>(cmd => cmd.Id == nonExistentEmployeeId));
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Employee with ID {nonExistentEmployeeId} not found for update.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteEmployee_ValidId_ReturnsOkWithDeletedId()
        {
            // Arrange
            int employeeIdToDelete = 5;

            _mediator.Send(Arg.Any<DeleteEmployeeCommand>()).Returns(1);

            // Act
            var result = await _controller.DeleteEmployee(employeeIdToDelete);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeIdToDelete));
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeletedId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(employeeIdToDelete, actualDeletedId);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteEmployee_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            int invalidId = -5;

            // Act
            var result = await _controller.DeleteEmployee(invalidId);

            // Assert
            await _mediator.DidNotReceive().Send(Arg.Any<DeleteEmployeeCommand>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Employee ID must be a positive integer.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteEmployee_EmployeeNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int nonExistentId = 999;

            _mediator.Send(Arg.Any<DeleteEmployeeCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await _controller.DeleteEmployee(nonExistentId);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<DeleteEmployeeCommand>(cmd => cmd.Id == nonExistentId));
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"Employee with ID {nonExistentId} not found for deletion.", notFoundResult.Value);
        }
    }
}