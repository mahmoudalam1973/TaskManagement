using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Models;

namespace TaskManagement.ViewModels
{
    public class CreateTaskViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [DisplayName("Task Status")]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

        public IFormFile? Attachment { get; set; }
    }
}
