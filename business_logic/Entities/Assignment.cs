namespace business_logic.Entities
{
    public enum RefreshType
    {
        Daily = 1,
        Weekly,
        Monthly
    }
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public IEnumerable<Step> Steps { get; set; }
        public RefreshType? Refresh { get; set; }
    }
}