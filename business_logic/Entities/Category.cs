namespace business_logic.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
    }
}
