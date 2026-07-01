using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.DTOs
{
    public class StepDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AssignmentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
