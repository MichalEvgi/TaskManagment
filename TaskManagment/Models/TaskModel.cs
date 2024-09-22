namespace TaskManagement.Models
{
    public enum TaskStatusModel
    {
        Pending,
        InProgress,
        Completed,
        Overdue
    }

    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskStatusModel Status { get; set; } = TaskStatusModel.Pending;
    }
}