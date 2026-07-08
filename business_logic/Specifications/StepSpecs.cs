using Ardalis.Specification;
using business_logic.Entities;
using Hangfire.Storage.Monitoring;

namespace business_logic.Specifications
{
    public class StepSpecs
    {

        public class ById : Specification<Step>
        {
            public ById(int id, string userId)
            {
                Query.Where(x => x.Assignment.UserId == userId);
            }
        }

        public class All : Specification<Step>
        {
            public All(string userId)
            {
                Query.Where(x => x.Assignment.UserId == userId);
            }
        }

        public class ByIdWithAssignment : Specification<Step>
        {
            public ByIdWithAssignment(int id, string userId)
            {
                Query.Where(x => x.AssignmentId == id && x.Assignment.UserId == userId).Include(x => x.Assignment);
            }
        }

        public class IncompleteStepsForAssignment : Specification<Step>
        {
            public IncompleteStepsForAssignment(int assignmentId, string userId)
            {
                Query.Where(s => s.AssignmentId == assignmentId && s.Assignment.UserId == userId)
                     .OrderBy(s => s.Id).Where(x => !x.IsCompleted);
            }
        }

    }
}
