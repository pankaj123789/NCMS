using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TitleMap : IAutoMappingOverride<Title>
    {
        public void Override(AutoMapping<Title> mapping)
        {
            mapping.Map(x => x.TitleName, "Title");
        }
    }
}
