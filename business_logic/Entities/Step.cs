namespace business_logic.Entities
{
    public class Step
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AssignmentId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public Assignment Assignment { get; set; }
    }
}
