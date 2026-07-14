namespace Core.DTOs
{
    public class PageParameters
    {
        private const int maxPageSize = 100;
        private int _pageSize = 10;

        public int pageNumber { get; set; } = 1;

        public int pageSize 
        { 
            get => _pageSize;
            set => _pageSize = (value <= 0) ? 10 : (value > maxPageSize ? maxPageSize : value);
        }

        public int? CategoryId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; } = false;
        public bool? IsImportant { get; set; }
    }

}
