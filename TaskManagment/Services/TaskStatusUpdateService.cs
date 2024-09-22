using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public class TaskStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public TaskStatusUpdateService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();

                    var tasksToUpdate = await context.Tasks
                        .Where(t => t.DueDate < DateTime.UtcNow &&
                                    (t.Status == TaskStatusModel.Pending || t.Status == TaskStatusModel.InProgress))
                        .ToListAsync(stoppingToken);

                    foreach (var task in tasksToUpdate)
                    {
                        task.Status = task.Status == TaskStatusModel.Pending ? TaskStatusModel.Overdue : TaskStatusModel.Completed;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}