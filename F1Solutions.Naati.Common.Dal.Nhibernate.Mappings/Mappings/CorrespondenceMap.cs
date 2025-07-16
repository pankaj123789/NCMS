using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class CorrespondenceMap : IAutoMappingOverride<Correspondence>
    {
        public void Override(AutoMapping<Correspondence> mapping)
        {
            mapping.HasMany<EntityCorrespondence>(Correspondence.Expressions.EntityCorrespondences)
                .Cascade.AllDeleteOrphan()
                .Inverse();


            mapping.SqlInsert("EXEC NH_CorrespondenceInsert ?,?,?,?,?,?,?,?").Check.None();
        }
    }
}
