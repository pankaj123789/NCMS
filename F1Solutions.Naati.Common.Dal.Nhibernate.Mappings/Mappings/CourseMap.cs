using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class CourseMap : IAutoMappingOverride<Course>
    {
        #region IAutoMappingOverride<Course> Members

        public void Override(AutoMapping<Course> mapping)
        {
            mapping.HasMany(x => x.CourseApprovals)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.Delete()
              .Inverse();

            mapping.HasMany(x => x.ChangeRequests).Inverse();
            
        }

        #endregion
    }


}

