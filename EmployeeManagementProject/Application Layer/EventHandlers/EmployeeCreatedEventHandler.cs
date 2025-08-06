using EmployeeManagementProject.Domain_Layer.Events;
using MediatR;

namespace EmployeeManagementProject.Application_Layer.EventHandlers
{
    public class EmployeeCreatedEventHandler : INotificationHandler<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeCreatedEventHandler> _logger;

        public EmployeeCreatedEventHandler(ILogger<EmployeeCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(EmployeeCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Employee Created Event: Employee with the Name '{notification._employee.Name}' was created. ");

            await Task.CompletedTask;
        }
    }
}