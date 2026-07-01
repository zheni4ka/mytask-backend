using Ardalis.Specification;
using business_logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.Specifications
{
    public class AssignmentSpecs
    {
        public class ById : Specification<Assignment>
        {
            public ById(int id)
            {
                Query.Where(a => a.Id == id);
            }
        }

        public class ByCategoryId : Specification<Assignment>
        {
            public ByCategoryId(int categoryId)
            {
                Query.Where(a => a.CategoryId == categoryId);
            }
        }

        public class LatestByCategoryId : Specification<Assignment>
        {
            public LatestByCategoryId(int categoryId)
            {
                Query.Where(a => a.CategoryId == categoryId)
                      .OrderByDescending(a => a.DueDate)
                      .Take(1);
            }
        }

        public class OverdueAssignments : Specification<Assignment>
        {
            public OverdueAssignments()
            {
                Query.Where(a => !a.IsCompleted && a.DueDate < DateTime.UtcNow)
                     .OrderBy(a => a.DueDate);
            }
        }

        public class UpcomingAssignments : Specification<Assignment>
        {
            public UpcomingAssignments(int daysAhead)
            {
                var futureDate = DateTime.UtcNow.AddDays(daysAhead);
                Query.Where(a => !a.IsCompleted && a.DueDate >= DateTime.UtcNow && a.DueDate <= futureDate)
                     .OrderBy(a => a.DueDate);
            }
        }
    }
}
