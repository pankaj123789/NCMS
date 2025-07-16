using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class EmailMap : IAutoMappingOverride<Email>
    {
        public void Override(AutoMapping<Email> mapping)
        {
            mapping.Map(x => x.EmailAddress, "Email");
            //mapping.References(x => x.Entity).Column("EntityId");
        }
    }
}
