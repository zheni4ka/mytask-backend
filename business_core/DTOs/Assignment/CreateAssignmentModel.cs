using Core.Entities;

namespace Core.DTOs
{
    public class CreateAssignmentModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime DueDate { get; set; }
        public RefreshType? RefreshType { get; set; }
        public bool IsImportant { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
