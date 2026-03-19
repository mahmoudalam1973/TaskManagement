using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

        public string? FileName { get; set; }      // original file name shown to user
        public string? StoredFileName { get; set; } // GUID-based name on disk

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum TaskItemStatus
    {
        Pending,
        InProgress,
        Completed
    }

}
