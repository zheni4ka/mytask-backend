using Ardalis.Specification;
using business_logic.Entities;

namespace business_logic.Specifications
{
    public class StepSpecs
    {
        public class ById : Specification<Step>
        {
            public ById(int id)
            {
                Query.Where(x => x.Id == id);
            }
        }

        public class ByIdWithAssignment : Specification<Step>
        {
            public ByIdWithAssignment(int id)
            {
                Query.Where(x => x.AssignmentId == id).Include(x => x.Assignment);
            }
        }

        public class IncompleteStepsForAssignment : Specification<Step>
        {
            public IncompleteStepsForAssignment(int assignmentId)
            {
                Query.Where(s => s.AssignmentId == assignmentId)
                     .OrderBy(s => s.Id).Where(x => !x.IsCompleted);
            }
        }

    }
}
