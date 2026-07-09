namespace Core.DTOs
{
    public class StepDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AssignmentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
