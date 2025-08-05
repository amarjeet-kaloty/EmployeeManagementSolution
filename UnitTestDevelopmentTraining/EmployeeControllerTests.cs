using EmployeeManagementProject.Application_Layer.Command.EmployeeCommands;
using EmployeeManagementProject.Application_Layer.Query.EmployeeQueries;
using EmployeeManagementProject.Domain_Layer.Entities;
using EmployeeManagementProject.Presentation_Layer.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace UnitTestDevelopmentTraining
{
    public class EmployeeControllerTests
    {
        [Fact]
        public async Task GetEmployeeList_ReturnListOfAllEmployees_SuccessAsync()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            var expectedEmployees = new List<Employee>
            {
                new Employee(
                    name: CreateUniqueTestEmployeeName("Test Employee", 1),
                    address: "123 Praline Ave",
                    email: "employee1@gmail.com",
                    phone: "404-111-1234"
                ) { Id = 1 },

                new Employee(
                    name: CreateUniqueTestEmployeeName("Test Employee", 2),
                    address: "456 Orange Lane",
                    email: "employee2@gmail.com",
                    phone: "505-000-7896"
                ) { Id = 2 }
            };

            mediator.Send(Arg.Any<GetEmployeeListQuery>()).Returns(expectedEmployees);

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.GetEmployeeList();

            // Assert
            await mediator.Received(1).Send(Arg.Any<GetEmployeeListQuery>());

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
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetEmployeeListQuery>()).Returns(new List<Employee>());

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.GetEmployeeList();

            // Assert
            await mediator.Received(1).Send(Arg.Any<GetEmployeeListQuery>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsEmployee()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            Employee expectedEmployee = new Employee
            (
                name: CreateUniqueTestEmployeeName("Test Employee", 1),
                address: "123 Praline Ave",
                email: "employee1@gmail.com",
                phone: "404-111-1234"
            )
            { Id = 1 };

            mediator.Send(Arg.Any<GetEmployeeByIdQuery>()).Returns(expectedEmployee);

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.GetEmployee(expectedEmployee.Id);

            // Assert
            await mediator.Received(1).Send(Arg.Any<GetEmployeeByIdQuery>());
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
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetEmployeeByIdQuery>()).Returns(Task.FromResult<Employee>(null));

            var controller = new EmployeeController(mediator);
            var nonExistentId = 101;

            // Act
            var result = await controller.GetEmployee(nonExistentId);

            // Assert
            await mediator.Received(1).Send(Arg.Any<GetEmployeeByIdQuery>());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Employee with ID {nonExistentId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task AddEmployee_ValidEmployee_ReturnsCreated()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            Employee newEmployee = new Employee
            (
                name: CreateUniqueTestEmployeeName("Test Employee", 1),
                address: "123 Praline Ave",
                email: "employee1@gmail.com",
                phone: "404-111-1234"
            )
            { Id = 1 };

            mediator.Send(Arg.Any<CreateEmployeeCommand>()).Returns(newEmployee);

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.AddEmployee(newEmployee);

            // Assert
            await mediator.Received(1).Send(Arg.Any<CreateEmployeeCommand>());
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(newEmployee.Id, actualEmployee.Id);
            Assert.Equal(newEmployee.Name, actualEmployee.Name);
            Assert.Equal(newEmployee.Address, actualEmployee.Address);
            Assert.Equal(newEmployee.Email, actualEmployee.Email);
            Assert.Equal(newEmployee.Phone, actualEmployee.Phone);
        }

        [Fact]
        public async Task AddEmployee_InValidEmployee_ReturnsBadRequest()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            Employee invalidEmployee = new Employee
             (
                 name: CreateUniqueTestEmployeeName("null", 0),
                 address: "123 Praline Ave",
                 email: null,
                 phone: "404-111-1234"
             )
            { Id = 0 };

            var controller = new EmployeeController(mediator);

            controller.ModelState.AddModelError(nameof(Employee.Name), "Name is required.");
            controller.ModelState.AddModelError(nameof(Employee.Email), "Invalid email format.");

            // Act
            var result = await controller.AddEmployee(invalidEmployee);

            // Assert
            await mediator.DidNotReceive().Send(Arg.Any<CreateEmployeeCommand>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value); 
            Assert.True(modelState.ContainsKey(nameof(Employee.Name)));
            Assert.Contains("Name is required.", modelState[nameof(Employee.Name)] as string[]);
            Assert.True(modelState.ContainsKey(nameof(Employee.Email)));
            Assert.Contains("Invalid email format.", modelState[nameof(Employee.Email)] as string[]);
        }

        [Fact]
        public async Task AddEmployee_MediatorReturnsNull_ReturnsInternalServerError()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<CreateEmployeeCommand>()).Returns(Task.FromResult<Employee>(null));

            var controller = new EmployeeController(mediator);

            Employee validEmployee = new Employee
            (
                name: CreateUniqueTestEmployeeName("Test Employee", 1),
               address: "456 Oak St",
               email: "employee1@gmail.com",
               phone: "404-111-1234"
            )
            { Id = 1 };

            // Act
            var result = await controller.AddEmployee(validEmployee);

            // Assert
            await mediator.Received(1).Send(Arg.Any<CreateEmployeeCommand>());
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Failed to create employee. An unexpected error occurred.", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateEmployee_ValidEmployee_ReturnsOkWithUpdatedId()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int employeeIdToUpdate = 1;
            Employee validEmployee = new Employee
            (
                name: CreateUniqueTestEmployeeName("Update Name", 1),
                address: "Updated Address",
                email: "updated@gmail.com",
                phone: "404-111-1234"
            )
            { Id = employeeIdToUpdate };

            mediator.Send(Arg.Any<UpdateEmployeeCommand>()).Returns(Task.FromResult(employeeIdToUpdate));

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.UpdateEmployee(validEmployee);

            // Assert
            await mediator.Received(1).Send(Arg.Is<UpdateEmployeeCommand>(cmd =>
                cmd.Id == validEmployee.Id &&
                cmd.Name == validEmployee.Name &&
                cmd.Address == validEmployee.Address &&
                cmd.Email == validEmployee.Email &&
                cmd.Phone == validEmployee.Phone
            ));
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualUpdatedId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(employeeIdToUpdate, actualUpdatedId);
        }

        [Fact]
        public async Task UpdateEmployee_InValidEmployee_ReturnsBadRequest()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int employeeIdToUpdate = 1;
            Employee invalidEmployee = new Employee
            (
                name: CreateUniqueTestEmployeeName(null, 0),
                address: "123 Praline Ave",
                email: null,
                phone: "404-111-1234"
            )
            { Id = employeeIdToUpdate };

            var controller = new EmployeeController(mediator);

            controller.ModelState.AddModelError(nameof(Employee.Name), "Name is required.");
            controller.ModelState.AddModelError(nameof(Employee.Email), "Invalid email format.");

            // Act
            var result = await controller.UpdateEmployee(invalidEmployee);

            // Assert
            await mediator.DidNotReceive().Send(Arg.Any<UpdateEmployeeCommand>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey(nameof(Employee.Name)));
            Assert.Contains("Name is required.", modelState[nameof(Employee.Name)] as string[]);
            Assert.True(modelState.ContainsKey(nameof(Employee.Email)));
            Assert.Contains("Invalid email format.", modelState[nameof(Employee.Email)] as string[]);
        }

        [Fact]
        public async Task UpdateEmployee_EmployeeNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int nonExistentEmployeeId = 999;
            Employee validEmployeeInput = new Employee
            (
               name: CreateUniqueTestEmployeeName("Test Employee", 1),
               address: "Valid Address",
               email: "valid@gmail.com",
               phone: "404-123-1234"
            )
            { Id = nonExistentEmployeeId };

            mediator.Send(Arg.Any<UpdateEmployeeCommand>()).Returns(Task.FromResult(0));

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.UpdateEmployee(validEmployeeInput);

            // Assert
            await mediator.Received(1).Send(Arg.Is<UpdateEmployeeCommand>(cmd => cmd.Id == nonExistentEmployeeId));
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Employee with ID {nonExistentEmployeeId} not found for update.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteEmployee_ValidId_ReturnsOkWithDeletedId()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int employeeIdToDelete = 5;

            mediator.Send(Arg.Any<DeleteEmployeeCommand>()).Returns(1);

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.DeleteEmployee(employeeIdToDelete);

            // Assert
            await mediator.Received(1).Send(Arg.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeIdToDelete));
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeletedId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(employeeIdToDelete, actualDeletedId);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteEmployee_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int invalidId = -5;

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.DeleteEmployee(invalidId);

            // Assert
            await mediator.DidNotReceive().Send(Arg.Any<DeleteEmployeeCommand>());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Employee ID must be a positive integer.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteEmployee_EmployeeNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            int nonExistentId = 999;

            mediator.Send(Arg.Any<DeleteEmployeeCommand>()).Returns(Task.FromResult(0));

            var controller = new EmployeeController(mediator);

            // Act
            var result = await controller.DeleteEmployee(nonExistentId);

            // Assert
            await mediator.Received(1).Send(Arg.Is<DeleteEmployeeCommand>(cmd => cmd.Id == nonExistentId));
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal($"Employee with ID {nonExistentId} not found for deletion.", notFoundResult.Value);
        }

        #region Helper Methods

        public EmployeeName CreateUniqueTestEmployeeName(string fullName, int uniqueIdentifier)
        {
            EmployeeName employeeName = new EmployeeName(fullName + uniqueIdentifier);
            return employeeName;
        }

        #endregion Helper Methods
    }
}