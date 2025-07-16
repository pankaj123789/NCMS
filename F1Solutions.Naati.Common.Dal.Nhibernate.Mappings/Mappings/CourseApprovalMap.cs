using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class CourseApprovalMap : IAutoMappingOverride<CourseApproval>
    {
        public void Override(AutoMapping<CourseApproval> mapping)
        {
            mapping.HasMany(x => x.ChangeRequestCourseApprovals)
                .Cascade.Delete()
                .Inverse();

            mapping.References(x => x.Course).Column("CourseId").Not.LazyLoad();
        }
    }
}
