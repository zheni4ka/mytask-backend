using business_logic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace data_access.Configs
{
    public class StepConfig : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.ToTable("Steps");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Assignment)
                   .WithMany(a => a.Steps)
                   .HasForeignKey(x => x.AssignmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
