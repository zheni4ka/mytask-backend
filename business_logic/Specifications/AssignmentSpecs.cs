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


    }
}
