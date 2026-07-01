using Ardalis.Specification;
using business_logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.Specifications
{
    public class CategorySpecs
    {
        public class ById : Specification<Category>
        {
            public ById(int id)
            {
                Query.Where(c => c.Id == id);
            }
        }

        public class ByName : Specification<Category>
        {
            public ByName(string name)
            {
                Query.Where(c => c.Name == name);
            }
        }

        public class ByLatestAssignments : Specification<Category>
        {
            public ByLatestAssignments()
            {
                Query.Include(c => c.Assignments)
                     .OrderByDescending(c => c.Assignments.Max(a => a.DueDate));
            }
        }
    }
}
