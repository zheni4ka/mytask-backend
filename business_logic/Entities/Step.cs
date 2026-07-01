namespace business_logic.Entities
{
    public class Step
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TaskId { get; set; }
        public Assignment Task { get; set; }
    }
}
