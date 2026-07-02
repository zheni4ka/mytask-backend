using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.DTOs
{
    public class PageParameters
    {
        private const int maxPageSize = 100;
        private int _pageSize = 10;

        public int pageNumber { get; set; } = 1;

        public int pageSize 
        { 
            get => _pageSize; 
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; } = false;
    }

}
