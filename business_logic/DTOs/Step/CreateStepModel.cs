namespace business_logic.DTOs
{
    public class CreateStepModel
    {
        public string Title { get; set; }
        public int AssignmentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
