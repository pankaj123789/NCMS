using F1Solutions.Naati.Common.Dal.Domain.Portal;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class MyNaatiUserMap : IAutoMappingOverride<MyNaatiUser>
    {
        public void Override(AutoMapping<MyNaatiUser> mapping)
        {
            mapping.Table("tblMyNaatiUser");
            mapping.Id(x => x.Id).Column("UserId");
        }
    }
}
