using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class RubricMarkingBandMap : IAutoMappingOverride<RubricMarkingBand>
    {
        public void Override(AutoMapping<RubricMarkingBand> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
