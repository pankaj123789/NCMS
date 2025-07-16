using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class SkillMap : IAutoMappingOverride<Skill>
    {
        public void Override(AutoMapping<Skill> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
            mapping.HasMany(x => x.CredentialRequests)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany(x => x.SkillApplicationTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }

    }
}
