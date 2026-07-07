using Ardalis.Specification;
using business_logic.DTOs;
using business_logic.Entities;

namespace business_logic.Specifications
{
    public class AssignmentSpecs
    {
        public class ById : Specification<Assignment>
        {
            public ById(int id, string userId)
            {
                Query.Where(a => a.Id == id && a.UserId == userId);
            }
        }

        public class ByCategoryId : Specification<Assignment>
        {
            public ByCategoryId(int categoryId, string userId)
            {
                Query.Where(a => a.CategoryId == categoryId && a.UserId == userId);
            }
        }

        public class LatestByCategoryId : Specification<Assignment>
        {
            public LatestByCategoryId(int categoryId, string userId)
            {
                Query.Where(a => a.CategoryId == categoryId && a.UserId == userId)
                      .OrderByDescending(a => a.DueDate)
                      .Take(1);
            }
        }

        public class OverdueAssignments : Specification<Assignment>
        {
            public OverdueAssignments(string userId)
            {
                Query.Where(a => !a.IsCompleted && a.DueDate < DateTime.UtcNow && a.UserId == userId)
                     .OrderBy(a => a.DueDate);
            }
        }

        public class UpcomingAssignments : Specification<Assignment>
        {
            public UpcomingAssignments(int daysAhead, string userId)
            {
                var futureDate = DateTime.UtcNow.AddDays(daysAhead);
                Query.Where(a => !a.IsCompleted && a.DueDate >= DateTime.UtcNow && a.DueDate <= futureDate && a.UserId == userId)
                     .OrderBy(a => a.DueDate);
            }
        }

        public class AssignmentsByQuerySpec : Specification<Assignment>
        {
            public AssignmentsByQuerySpec(PageParameters pageParameters, string userId)
            {
                Query.Where(a => a.UserId == userId);

                if (!string.IsNullOrEmpty(pageParameters.SearchTerm))
                {
                    Query.Where(a => a.Title.Contains(pageParameters.SearchTerm) || a.Description.Contains(pageParameters.SearchTerm));
                }

                if (!string.IsNullOrEmpty(pageParameters.SortBy))
                {
                    switch(pageParameters.SortBy)
                    {
                        case "title":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.Title).Where(a => a.UserId == userId);
                            else Query.OrderBy(a => a.Title).Where(a => a.UserId == userId);
                            break;

                        case "description":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.Description).Where(a => a.UserId == userId);
                            else Query.OrderBy(a => a.Description).Where(a => a.UserId == userId);
                            break;

                        case "duedate":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.DueDate).Where(a => a.UserId == userId);
                            else Query.OrderBy(a => a.DueDate).Where(a => a.UserId == userId);
                            break;
                    }
                }

                Query.Skip((pageParameters.pageNumber - 1) * pageParameters.pageSize)
                     .Take(pageParameters.pageSize);
            }
        }

        public class AssignmentsByQueryCountSpec : Specification<Assignment>
        {
            public AssignmentsByQueryCountSpec(PageParameters pageParameters, string userId)
            {
                if (!string.IsNullOrEmpty(pageParameters.SearchTerm))
                {
                    Query.Where(a => a.Title.Contains(pageParameters.SearchTerm) ||
                                     a.Description.Contains(pageParameters.SearchTerm) && a.UserId == userId);
                }
            }
        }

    }
}
