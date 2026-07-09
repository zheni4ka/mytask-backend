namespace Core.DTOs
{
    public class EditStepModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AssignmentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
