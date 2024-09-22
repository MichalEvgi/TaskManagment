using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using TaskManagement.Controllers;
using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Tests
{
    public class TasksControllerTests
    {
        private TaskManagementContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<TaskManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new TaskManagementContext(options);
            return context;
        }

        [Fact]
        public async Task GetTasks_ReturnsAllTasks()
        {
            // Arrange
            var context = GetDatabaseContext();
            context.Tasks.AddRange(
                new TaskModel { Title = "Task 1", Description = "Task 1", Status = TaskStatusModel.Pending },
                new TaskModel { Title = "Task 2", Description = "Task 2", Status = TaskStatusModel.InProgress }
            );
            context.SaveChanges();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTasks();

            // Assert
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskModel>>(result.Value);
            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public async Task GetTask_ReturnsCorrectTask()
        {
            // Arrange
            var context = GetDatabaseContext();
            var task = new TaskModel { Id = 1, Title = "Task 1", Description = "Task 1", Status = TaskStatusModel.Pending };
            context.Tasks.Add(task);
            context.SaveChanges();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTask(1);

            // Assert
            var returnedTask = Assert.IsType<TaskModel>(result.Value);
            Assert.Equal(task.Id, returnedTask.Id);
            Assert.Equal(task.Title, returnedTask.Title);
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTask(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostTask_CreatesNewTask()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new TasksController(context);
            var newTask = new TaskModel { Title = "new TaskModel", Status = TaskStatusModel.Pending };

            // Act
            var result = await controller.PostTask(newTask);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTask = Assert.IsType<TaskModel>(createdAtActionResult.Value);
            Assert.Equal(newTask.Title, returnedTask.Title);
            Assert.Equal(1, context.Tasks.Count());
        }

        [Fact]
        public async Task PutTask_UpdatesExistingTask()
        {
            // Arrange
            var context = GetDatabaseContext();
            var task = new TaskModel { Id = 1, Title = "Task 1", Status = TaskStatusModel.Pending };
            context.Tasks.Add(task);
            context.SaveChanges();
            var controller = new TasksController(context);
            var updatedTask = new TaskModel { Id = 1, Title = "Updated Task", Status = TaskStatusModel.InProgress };

            // Act
            var result = await controller.PutTask(1, updatedTask);

            // Assert
            Assert.IsType<NoContentResult>(result);
            context.Entry(task).Reload(); // Reload the entity from the database
            Assert.Equal(updatedTask.Title, task.Title);
            Assert.Equal(updatedTask.Status, task.Status);
        }

        [Fact]
        public async Task PutTask_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new TasksController(context);
            var task = new TaskModel { Id = 2, Title = "Task", Status = TaskStatusModel.Pending };

            // Act
            var result = await controller.PutTask(1, task);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new TasksController(context);
            var task = new TaskModel { Id = 1, Title = "Task", Status = TaskStatusModel.Pending };

            // Act
            var result = await controller.PutTask(1, task);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTask_RemovesTask()
        {
            // Arrange
            var context = GetDatabaseContext();
            var task = new TaskModel { Id = 1, Title = "Task to Delete", Status = TaskStatusModel.Pending };
            context.Tasks.Add(task);
            context.SaveChanges();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Tasks.FindAsync(1));
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}