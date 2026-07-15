using Core.Entities;

using Core.Entities;

namespace Core.DTOs
{
    public class AssignmentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
        public RefreshType? RefreshType { get; set; }
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public bool IsImportant { get; set; } 
        public bool IsCompleted { get; set; }
        public string? GoogleEventId { get; set; }
    }
}
