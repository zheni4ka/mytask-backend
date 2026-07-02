using Ardalis.Specification;
using business_logic.DTOs;
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

        public class AssignmentsByQuerySpec : Specification<Assignment>
        {
            public AssignmentsByQuerySpec(PageParameters pageParameters)
            {
                if (!string.IsNullOrEmpty(pageParameters.SearchTerm))
                {
                    Query.Where(a => a.Title.Contains(pageParameters.SearchTerm) || a.Description.Contains(pageParameters.SearchTerm));
                }

                if (!string.IsNullOrEmpty(pageParameters.SortBy))
                {
                    switch(pageParameters.SortBy)
                    {
                        case "title":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.Title);
                            else Query.OrderBy(a => a.Title);
                            
                            break;
                        case "description":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.Description);
                            else Query.OrderBy(a => a.Description);
                            
                            break;
                        case "duedate":
                            
                            if(pageParameters.SortDescending) Query.OrderByDescending(a => a.DueDate);
                            else Query.OrderBy(a => a.DueDate);
                            
                            break;
                    }
                }

                Query.Skip((pageParameters.pageNumber - 1) * pageParameters.pageSize)
                     .Take(pageParameters.pageSize);
            }
        }

    }
}
