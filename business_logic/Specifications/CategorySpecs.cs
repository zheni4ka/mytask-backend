using Ardalis.Specification;
using Core.Entities;

namespace business_logic.Specifications
{
    public class CategorySpecs
    {
        public class ById : Specification<Category>
        {
            public ById(int id, string userId)
            {
                Query.Where(c => c.Id == id && c.UserId == userId);
            }
        }

        public class All : Specification<Category>
        {
            public All(string userId)
            {
                Query.Where(c => c.UserId == userId);
            }
        }

        public class ByName : Specification<Category>
        {
            public ByName(string name, string userId)
            {
                Query.Where(c => c.Name == name && c.UserId == userId);
            }
        }

        public class ByLatestAssignments : Specification<Category>
        {
            public ByLatestAssignments(int categoryId, string userId)
            {
                Query.Where(c => c.Id == categoryId && c.UserId == userId)
                     .Include(c => c.Assignments)
                     .OrderByDescending(c => c.Assignments.Max(a => a.DueDate));
            }
        }

        public class CategoriesWithActiveTasks : Specification<Category>
        {
            public CategoriesWithActiveTasks(string userId)
            {
                Query.Where(c => c.Assignments.Any(a => !a.IsCompleted && a.UserId == userId))
                     .OrderBy(c => c.Name);
            }
        }

        public class EmptyCategories : Specification<Category>
        {
            public EmptyCategories(string userId)
            {
                Query.Where(c => !c.Assignments.Any(a => a.UserId == userId));
            }
        }
    }
}
