using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Tests
{
    public class TaskStatusUpdateServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_UpdatesOverdueTasks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TaskManagementContext>()
                .UseInMemoryDatabase(databaseName: "ExecuteAsync_UpdatesOverdueTasks")
                .Options;

            using (var context = new TaskManagementContext(options))
            {
                context.Tasks.Add(new TaskModel { Id = 1, Title = "Overdue Task", Description = "Overdue Task", Status = TaskStatusModel.Pending, DueDate = DateTime.UtcNow.AddDays(-1) });
                context.Tasks.Add(new TaskModel { Id = 2, Title = "Future Task", Description = "Future Task", Status = TaskStatusModel.Pending, DueDate = DateTime.UtcNow.AddDays(1) });
                context.Tasks.Add(new TaskModel { Id = 3, Title = "Completed Task", Description = "Completed Task", Status = TaskStatusModel.InProgress, DueDate = DateTime.UtcNow.AddDays(-1) });
                context.SaveChanges();
            }

            var services = new ServiceCollection();
            services.AddDbContext<TaskManagementContext>(opt => opt.UseInMemoryDatabase("ExecuteAsync_UpdatesOverdueTasks"));
            var serviceProvider = services.BuildServiceProvider();

            var service = new TaskStatusUpdateService(serviceProvider);

            // Act
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(100); // Give some time for the background service to run
            await service.StopAsync(CancellationToken.None);

            // Assert
            using (var context = new TaskManagementContext(options))
            {
                var overdueTask = await context.Tasks.FindAsync(1);
                var futureTask = await context.Tasks.FindAsync(2);
                var completeTask = await context.Tasks.FindAsync(3);

                Assert.Equal(TaskStatusModel.Overdue, overdueTask.Status);
                Assert.Equal(TaskStatusModel.Pending, futureTask.Status);
                Assert.Equal(TaskStatusModel.Completed, completeTask.Status);
            }
        }
    }
}