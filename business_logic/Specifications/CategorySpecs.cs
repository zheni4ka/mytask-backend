using Ardalis.Specification;
using business_logic.Entities;

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
            public ByLatestAssignments(int categoryId)
            {
                Query.Where(c => c.Id == categoryId)
                     .Include(c => c.Assignments)
                     .OrderByDescending(c => c.Assignments.Max(a => a.DueDate));
            }
        }

        public class CategoriesWithActiveTasks : Specification<Category>
        {
            public CategoriesWithActiveTasks()
            {
                Query.Where(c => c.Assignments.Any(a => !a.IsCompleted))
                     .OrderBy(c => c.Name);
            }
        }

        public class EmptyCategories : Specification<Category>
        {
            public EmptyCategories()
            {
                Query.Where(c => !c.Assignments.Any());
            }
        }
    }
}
